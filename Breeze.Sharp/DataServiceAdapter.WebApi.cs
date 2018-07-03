﻿using Breeze.Sharp.Core;
//using Microsoft.Data.OData;
using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Breeze.Sharp {

  /// <summary>
  /// IDataServiceAdapter implementation that supports Microsoft's WebApi.
  /// </summary>
  public class WebApiDataServiceAdapter : IDataServiceAdapter {

    public String Name {
      get { return "WebApi"; }
    }

    public IJsonResultsAdapter JsonResultsAdapter {
      get { return WebApiJsonResultsAdapter.Instance; }
    }

    public async Task<SaveResult> SaveChanges(IEnumerable<IEntity> entitiesToSave, SaveOptions saveOptions) {
      var entityManager = entitiesToSave.First().EntityAspect.EntityManager;
      var saveBundleNode = PrepareSaveBundle(entitiesToSave, saveOptions);
      try {
        var saveResultJson =
          await saveOptions.DataService.PostAsync(saveOptions.ResourceName, saveBundleNode.Serialize());
        var jn = JNode.DeserializeFrom(saveResultJson);
        if (jn.HasValues("Errors") || jn.HasValues("errors")) {
          throw new SaveException(entityManager, jn);
        }
        return ProcessSaveResult(entityManager, saveOptions, jn);
      } catch (DataServiceRequestException dsre) {
        var jn = JNode.DeserializeFrom(dsre.ResponseContent);
        throw new SaveException(entityManager, jn);
      } catch (HttpRequestException e) {
        throw new SaveException(e.Message, e);
      }
    }

    #region Save Preparation

    private JNode PrepareSaveBundle(IEnumerable<IEntity> entitiesToSave, SaveOptions saveOptions) {
      var jn = new JNode();
      jn.AddArray("entities", entitiesToSave.Select(e => EntityToJNode(e)));
      jn.AddJNode("saveOptions", saveOptions);
      return jn;
    }

    private JNode EntityToJNode(IStructuralObject so) {
      var entity = so as IEntity;
      JNode jn;
      if (entity != null) {
        var entityAspect = entity.EntityAspect;
        jn = DataToJNode(entityAspect);
        jn.AddJNode("entityAspect", BuildEntityAspectNode(entityAspect));
      } else {
        var complexAspect = ((IComplexObject)so).ComplexAspect;
        jn = DataToJNode(complexAspect);
      }
      return jn;      
    }

    private JNode DataToJNode(StructuralAspect aspect) {
      var jn = new JNode();
      var stype = aspect.StructuralType;

      stype.DataProperties.ForEach(dp => {
        var val = aspect.GetValue(dp.Name);
        // handle nonscalar dps
        if (dp.IsComplexProperty) {
          jn.AddJNode(dp.NameOnServer, DataToJNode(((IComplexObject)val).ComplexAspect));
        } else {
          // always add the val even if equal to the default
          jn.AddPrimitive(dp.NameOnServer, val);
        }
      });
      return jn;
    }

    private JNode BuildEntityAspectNode(EntityAspect entityAspect) {
      var nc = entityAspect.EntityManager.MetadataStore.NamingConvention;
      var jn = new JNode();
      var entityType = entityAspect.EntityType;
        
      jn.AddPrimitive("entityTypeName", entityType.NameOnServer);
      jn.AddEnum("entityState", entityAspect.EntityState);
      jn.AddPrimitive("defaultResourceName", entityType.DefaultResourceName);
      jn.AddJNode("originalValuesMap", BuildOriginalValuesMapNode(entityAspect, nc));
      var agkType = entityType.AutoGeneratedKeyType;
      if (agkType != AutoGeneratedKeyType.None) {
        var agkNode = new JNode();
        agkNode.AddPrimitive("propertyName", entityType.KeyProperties[0].Name);
        agkNode.AddEnum("autoGeneratedKeyType", agkType);
        jn.AddJNode("autoGeneratedKey", agkNode);
      }
      return jn;
    }

    private JNode BuildOriginalValuesMapNode(StructuralAspect aspect, NamingConvention nc) {
      var stType = aspect.StructuralType;
      var ovMap = aspect.OriginalValuesMap.ToDictionary(
        kvp => stType.GetProperty(kvp.Key).NameOnServer, 
        kvp => kvp.Value);      
      var cps = stType.ComplexProperties;        
      cps.ForEach(cp => {
        var co = aspect.GetValue(cp.Name);
        var serverName = cp.NameOnServer;
        if (cp.IsScalar) {
          var ovmNode = BuildOriginalValuesMapNode( ((IComplexObject) co).ComplexAspect, nc);
          ovMap[serverName] = ovmNode;
        } else {
          var ovmNodes = ((IEnumerable) co).Cast<IComplexObject>().Select(co2 => BuildOriginalValuesMapNode(co2.ComplexAspect, nc));
          ovMap[serverName] = JNode.ToJArray(ovmNodes);
        }
      });
      var result = JNode.BuildMapNode(ovMap);
      return result;
    }

    #endregion

    #region Save results processing

    private SaveResult ProcessSaveResult(EntityManager entityManager, SaveOptions saveOptions, JNode jNode) {

      var kms = jNode.GetArray<KeyMapping>("KeyMappings");
      var keyMappings = kms.Select(km => ToEntityKeys(km, entityManager.MetadataStore)).ToDictionary(tpl => tpl.Item1, tpl => tpl.Item2);
      using (entityManager.NewIsLoadingBlock(false)) {
        keyMappings.ForEach(km => {
          var targetEntity = entityManager.GetEntityByKey(km.Key);
          targetEntity.EntityAspect.SetDpValue(km.Key.EntityType.KeyProperties[0], km.Value.Values[0]);
        });

        var entityNodes = jNode.GetToken<JArray>("Entities");

        var serializer = new JsonSerializer();
        var mappingContext = new MappingContext() {
          EntityManager = entityManager,
          MergeStrategy = MergeStrategy.OverwriteChanges,
          LoadingOperation = LoadingOperation.Save,
          JsonResultsAdapter = saveOptions.DataService.JsonResultsAdapter
        };
        var jsonConverter = new JsonEntityConverter(mappingContext);
        serializer.Converters.Add(jsonConverter);
        // serializer.Converters.Add(new StringEnumConverter());
        // Don't use the result of the Deserialize call to get the list of entities 
        // because it won't include entities added on the server.
        serializer.Deserialize<IEnumerable<IEntity>>(entityNodes.CreateReader());
        var allEntities = mappingContext.Entities;
        allEntities.ForEach(e => e.EntityAspect.AcceptChanges());
        return new SaveResult(allEntities, new ConcurrentDictionary<EntityKey, EntityKey>(keyMappings));
      }

    }

    private Tuple<EntityKey, EntityKey> ToEntityKeys(KeyMapping keyMapping, MetadataStore metadataStore) {
      var serverTypeInfo = TypeNameInfo.FromClrTypeName(keyMapping.EntityTypeName);
      var clientEntityTypeName = serverTypeInfo.ToClient(metadataStore).StructuralTypeName;
      var et = metadataStore.GetEntityType(clientEntityTypeName);
      var oldKey = new EntityKey(et, keyMapping.TempValue);
      var newKey = new EntityKey(et, keyMapping.RealValue);
      return Tuple.Create(oldKey, newKey);
    }


    #endregion
  }

  public class KeyMapping {
    public String EntityTypeName;
    public Object TempValue;
    public Object RealValue;
  }

  public class WebApiJsonResultsAdapter : IJsonResultsAdapter {

    public static WebApiJsonResultsAdapter Instance = new WebApiJsonResultsAdapter();

    public string Name {
      get { return "WebApiDefault"; }
    }

    public JToken ExtractResults(JToken node) {
      return node;
    }

    public JsonNodeInfo VisitNode(JObject node, MappingContext mappingContext, NodeContext nodeContext) {
      var result = new JsonNodeInfo();

      JToken refToken;
      if (node.TryGetValue("$ref", out refToken)) {
        result.NodeRefId = refToken.Value<String>();
        return result;
      }

      JToken idToken;
      if (node.TryGetValue("$id", out idToken)) {
        result.NodeId = idToken.Value<String>();
      }

      JToken typeToken;
      if (node.TryGetValue("$type", out typeToken)) {
        var clrTypeName = typeToken.Value<String>();
        result.ServerTypeNameInfo = TypeNameInfo.FromClrTypeName(clrTypeName);
      }

      return result;
    }
  }

  


 
}

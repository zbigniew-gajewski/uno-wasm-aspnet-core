namespace UnoTest.Web.Data
{
    using Breeze.Sharp;
    using System.ComponentModel.DataAnnotations;

    public class Customer : BaseEntity
    {
        public int Id
        {
            get
            {
                return EntityAspect.GetValue<int>("Id");
            }
            set
            {
                EntityAspect.SetValue("Id", value);
            }
        }

        //[Required, StringLength(maximumLength: 30)]    // Validation rules
        public string FirstName                      // "Get milk"
        {
            get
            {
                return EntityAspect.GetValue<string>("FirstName"); 
            }
            set
            {
                EntityAspect.SetValue("FirstName", value);
            }
        }

        public string LastName                      // "Get milk"
        {
            get
            {
                return EntityAspect.GetValue<string>("LastName");  
            }
            set
            {
                EntityAspect.SetValue("LastName", value);
            }
        }

        public string Description                      // "Get milk"
        {
            get
            {
                return EntityAspect.GetValue<string>("Description");
            }
            set
            {
                //SetValue(value);
                EntityAspect.SetValue("Description", value);
            }
        }
    }
}

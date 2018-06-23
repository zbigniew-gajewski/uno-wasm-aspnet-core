namespace UnoTest.Web.Data
{
    using Breeze.Sharp;
    using System.ComponentModel.DataAnnotations;

    public class Customer : BaseEntity
    {
        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        //[Required, StringLength(maximumLength: 30)]    // Validation rules
        public string FirstName                      // "Get milk"
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LastName                      // "Get milk"
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}

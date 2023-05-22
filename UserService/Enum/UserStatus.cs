using System;
namespace CatalogTest.UserStatus
{
	public enum UserStatus
	{
		[StatusValue(false)]
		Nonactive,
        [StatusValue(true)]
		Active


    }

	public class StatusValueAttribute : Attribute
	{
		public bool Value { get; }

		public StatusValueAttribute(bool value)
		{
			Value = value;
		}
	}
}


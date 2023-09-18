#nullable disable
namespace ASK.API.Helpers
{
	public class PhoneNumberFormater
	{
		// Kenyan phone format
		public static string FormatThePhoneNumber(string phoneNumber)
		{
			string prefix = "254";

			if (phoneNumber.Length < 9)
			{
				return null;
			}

			if (string.IsNullOrEmpty(phoneNumber)) throw new ArgumentNullException("Phone number is null");

			if (phoneNumber.StartsWith('0') && phoneNumber.Length == 10)
			{
				return $"{prefix}{phoneNumber.Substring(1)}";
			}

			if (phoneNumber.StartsWith('+'))
			{
				if (phoneNumber.Substring(1, 3) == prefix)
				{
					return phoneNumber.Length == 13 ? phoneNumber.Substring(1) : null;
				}

				return null;
			}

			if (phoneNumber.Substring(0, 3) == prefix && phoneNumber.Length == 12)
			{
				return phoneNumber;
			}

			if (!phoneNumber.StartsWith('0') && phoneNumber.Length == 9)
			{
				return $"{prefix}{phoneNumber}";
			}

			return null;
		}

		public static string FormatAirtelPhoneNumber(string phoneNumber)
		{
			string prefix = "254";

			if (phoneNumber.Length < 9)
			{
				return null;
			}

			if (string.IsNullOrEmpty(phoneNumber)) return null;
			//0717731993
			if (phoneNumber.StartsWith('0') && phoneNumber.Length == 10)
			{
				return phoneNumber.Substring(1);
			}
			//+254717731993
			if (phoneNumber.StartsWith('+'))
			{
				if (phoneNumber.Substring(1, 3) == prefix)
				{
					return phoneNumber.Length == 13 ? phoneNumber.Substring(4) : null;
				}

				return null;
			}
			//254717731992
			if (phoneNumber.Substring(0, 3) == prefix && phoneNumber.Length == 12)
			{
				return phoneNumber.Substring(3);
			}

			if (!phoneNumber.StartsWith('0') && phoneNumber.Length == 9)
			{
				return phoneNumber;
			}

			return null;
		}
	}
}

using System.ComponentModel.DataAnnotations;

namespace Shared.Models.CustomValidations
{
    public sealed class NoThreeOrMoreSpacesInARow : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string input = value.ToString();

            bool noThreeOrMoreSpacesInARow = true;

			for (int i = 2; i < input.Length; i++)
			{
				if (input[i] == ' ' && input[i] == input[i - 1] && input[i] == input[i - 2])
				{
                    noThreeOrMoreSpacesInARow = false;
				}
			}

            return noThreeOrMoreSpacesInARow;
        }
    }
}

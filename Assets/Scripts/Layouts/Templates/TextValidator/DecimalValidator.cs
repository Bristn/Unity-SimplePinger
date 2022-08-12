public class DecimalValidator : CustomValidator
{
    private float minValue;
    private float maxValue;
    private bool integer;

    public DecimalValidator(float pMinValue, float pMaxValue, bool pIntegerOnly = false)
    {
        minValue = pMinValue;
        maxValue = pMaxValue;
        integer = pIntegerOnly;
    }

    public override bool IsValidValue(string pValue)
    {
        if (integer)
        {
            bool valid = int.TryParse(pValue, out int numberValue);
            if (valid)
            {
                return numberValue >= minValue && numberValue <= maxValue;
            }
            return valid;
        }
        else
        {
            bool valid = float.TryParse(pValue, out float numberValue);
            if (valid)
            {
                return numberValue >= minValue && numberValue <= maxValue;
            }
            return valid;
        }
    }
}
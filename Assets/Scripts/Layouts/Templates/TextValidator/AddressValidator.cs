using System;

public class AddressValidator : CustomValidator
{
    private DecimalValidator decimalValidator;

    public AddressValidator()
    {
        decimalValidator = new DecimalValidator(0, 255, true);
    }

    public override bool IsValidValue(string pValue)
    {
        return IsValidIp(pValue) || IsValidUri(pValue);
    }

    private bool IsValidIp(string pValue)
    {
        string[] subValues = pValue.Split(".");
        if (subValues.Length != 4)
        {
            return false;
        }

        foreach (string value in subValues)
        {
            if (!decimalValidator.IsValidValue(value))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsValidUri(string pValue)
    {
        try
        {
            Uri temp = new Uri(pValue);
            return true;
        }
        catch (Exception) { }

        return false;
    }
}

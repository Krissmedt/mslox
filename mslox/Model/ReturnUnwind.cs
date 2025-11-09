public class ReturnUnwind : SystemException
{
    public object Value { get; }

    public ReturnUnwind(object value)
    {
        this.Value = value;
    }
}
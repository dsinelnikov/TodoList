namespace TodoListApi.ExceptionHandling
{
    public enum SqlExceptionType
    {
        None,
        Constraint,
        DublicateKey,
        Concurrency
    }
}

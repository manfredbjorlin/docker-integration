public class ServiceBusToServiceMapper
{
    public ServiceBusToServiceMapper()
    {
        
    }

    public string Map(PersonExample person)
    {
        return $"{person.Firstname} {person.Lastname}";
    }
}
public record PersonExample(string Firstname, string Lastname, string DOB, int age, AddressExample address);

public record AddressExample(string street, string zipcode, string city);
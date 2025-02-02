
public class PatientGenerator
{
    private readonly Random _random = new Random();
    private readonly string[] _genders = { "unknown", "male", "female", "other" };
    private readonly string[] _familyNames = { "Иванов", "Лунев", "Коркин", "Филев", "Ложкин", "Сидкович", "Пирожков", "Давыдович" };
    private readonly string[] _givenNames = { "Андрей", "Иван", "Мария", "Александр", "Степан", "Юлия", "Евгения", "Михаил" };
    private readonly string[] _givenByFatherNames = { "Иванович", "Александрович", "Петрович", "Сергеевич", "Никитович", "Степанович", "Михаилович" };

    public List<Patient> Generate(int count)
    {
        var patients = Enumerable.Range(1, count).Select(_ => new Patient()
        {
            Name = GenerateName(),
            Gender = _genders[_random.Next(_genders.Length)],
            BirthDate = GenerateBirthDate(),
            Active = _random.Next(0, 2) == 0
        }).ToList();
        
        return patients;
    }

    private Name GenerateName()
    {
        Name name = new Name();
        name.Family = _familyNames[_random.Next(_familyNames.Length)];
        name.Given.Add(_givenNames[_random.Next(_givenNames.Length)]);
        name.Given.Add(_givenByFatherNames[_random.Next(_givenByFatherNames.Length)]);
        return name;
    }

    private DateTime GenerateBirthDate()
    {
        int year = _random.Next(1920, 2025);
        int month = _random.Next(1, 13);
        int day = _random.Next(1, DateTime.DaysInMonth(year, month) + 1);
        int hour = _random.Next(1, 24);
        int minute = _random.Next(1, 60);
        int second = _random.Next(1, 60);
        return new DateTime(year, month, day, hour, minute, second).ToUniversalTime();

    }

}
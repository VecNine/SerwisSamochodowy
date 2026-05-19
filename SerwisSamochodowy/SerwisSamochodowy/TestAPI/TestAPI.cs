using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public class TestAPI
{
    /*
     * Aby uruchomić, trzeba skopiować obecną klasę do innego programu
     * oraz wywołać poniższe komendy
     * 
     * var apiTester = new TestAPI();
     * await apiTester.RunTestsAsync();
     */
    private readonly HttpClient _client;
    private readonly CreatedIds _ids;
    
    public TestAPI()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:5194/") };
        _client.DefaultRequestHeaders.Add("X-Username", "admin");
        _client.DefaultRequestHeaders.Add("X-Api-Token", "76be39f1-54f3-4d8d-8939-fb691ae5b3ec");

        _ids = new CreatedIds { MechId = 9, PartId = 11, VehId = 9, RepId = 11 };
    }
    
    public async Task RunTestsAsync()
    {
        Console.WriteLine("--- START PROGRAMU TESTUJĄCEGO ---\n");

        // 1. Dodawanie
        await TestDodawaniaAsync();

        // 2. Edycja
        // await TestEdycjiAsync();

        // 3. Usuwanie
        // await TestUsuwaniaAsync();

        Console.WriteLine("\n--- KONIEC PROGRAMU ---");
    }

    private async Task TestDodawaniaAsync()
    {
        Console.WriteLine(">>> URUCHAMIAM: Test Dodawania (POST)");
        try
        {
            // Mechanik
            var mRes = await _client.PostAsJsonAsync("Mechanic/api/add",
                new { Name = "Robert", Surname = "Kubica", Salary = 20000m, EmploymentStartDate = DateTime.Now });
            var mJson = await mRes.Content.ReadFromJsonAsync<JsonElement>();
            _ids.MechId = mJson.GetProperty("idMechanic").GetInt32();
            Console.WriteLine($"[OK] Dodano mechanika ID: {_ids.MechId}");

            // Część
            var pRes = await _client.PostAsJsonAsync("Parts/api/add", new { Name = "Opona", Cost = 400.00m });
            var pJson = await pRes.Content.ReadFromJsonAsync<JsonElement>();
            _ids.PartId = pJson.GetProperty("id").GetInt32();
            Console.WriteLine($"[OK] Dodano część ID: {_ids.PartId}");

            // Pojazd
            var vRes = await _client.PostAsJsonAsync("Vehicle/api/add",
                new
                {
                    Model = "BMW M3", Year = 2020, OwnerName = "Krzysztof", OwnerSurname = "Hołowczyc",
                    OwnerPhone = 555666777
                });
            var vJson = await vRes.Content.ReadFromJsonAsync<JsonElement>();
            _ids.VehId = vJson.GetProperty("idVehicle").GetInt32();
            Console.WriteLine($"[OK] Dodano pojazd ID: {_ids.VehId}");

            // Naprawa
            var rRes = await _client.PostAsJsonAsync("Repairment/api/add",
                new { Date = DateTime.Now, ServicePrice = 1200m, IdMechanic = _ids.MechId, IdVehicle = _ids.VehId });
            var rJson = await rRes.Content.ReadFromJsonAsync<JsonElement>();
            _ids.RepId = rJson.GetProperty("idRepairment").GetInt32();
            Console.WriteLine($"[OK] Dodano naprawę ID: {_ids.RepId}");

            Console.WriteLine(">>> Przypisywanie opony do naprawy...");
            var rpRes = await _client.PostAsJsonAsync("Repairment/api/addPart", new
            {
                IdRepairment = _ids.RepId,
                IdPart = _ids.PartId,
                Quantity = 1
            });

            if (rpRes.IsSuccessStatusCode)
                Console.WriteLine("[OK] Opona została pomyślnie dodana do naprawy!");
            else
                Console.WriteLine("[!] Błąd podczas przypisywania części do naprawy.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Błąd dodawania: {ex.Message}");
        }
    }

    private async Task TestEdycjiAsync()
    {
        if (_ids.MechId == 0 || _ids.PartId == 0 || _ids.VehId == 0 || _ids.RepId == 0)
        {
            Console.WriteLine(
                "[!] Uwaga: Brak ID do edycji. Uruchom najpierw TestDodawaniaAsync() lub ustaw poprawne ID w obiekcie '_ids'.");
            return;
        }

        Console.WriteLine(
            $">>> URUCHAMIAM: Test Edycji (PUT) dla ID: M:{_ids.MechId}, P:{_ids.PartId}, V:{_ids.VehId}, R:{_ids.RepId}");

        // Edycja Części
        var pUpd = await _client.PutAsJsonAsync($"Parts/api/update/{_ids.PartId}",
            new { Name = "Opona Zimowa Premium", Cost = 480.00m });
        Console.WriteLine(pUpd.IsSuccessStatusCode ? "[OK] Edycja części udana" : "[!] Błąd edycji części");

        // Edycja Mechanika
        var mUpd = await _client.PutAsJsonAsync($"Mechanic/api/update/{_ids.MechId}",
            new { Name = "Robert", Surname = "Kubica-Giga", Salary = 25000m, EmploymentStartDate = DateTime.Now });
        Console.WriteLine(mUpd.IsSuccessStatusCode ? "[OK] Edycja mechanika udana" : "[!] Błąd edycji mechanika");

        // Edycja Pojazdu
        var vUpd = await _client.PutAsJsonAsync($"Vehicle/api/update/{_ids.VehId}",
            new
            {
                Model = "BMW M3 Competition", Year = 2021, OwnerName = "Krzysztof", OwnerSurname = "Hołowczyc",
                OwnerPhone = "555666777"
            });
        Console.WriteLine(vUpd.IsSuccessStatusCode ? "[OK] Edycja pojazdu udana" : "[!] Błąd edycji pojazdu");

        // Edycja Zlecenia Naprawy
        var rUpd = await _client.PutAsJsonAsync($"Repairment/api/update/{_ids.RepId}",
            new { Date = DateTime.Now, ServicePrice = 1500m, IdMechanic = _ids.MechId, IdVehicle = _ids.VehId });
        Console.WriteLine(rUpd.IsSuccessStatusCode
            ? "[OK] Edycja zlecenia naprawy udana"
            : "[!] Błąd edycji zlecenia naprawy");
    }

    private async Task TestUsuwaniaAsync()
    {
        if (_ids.MechId == 0 || _ids.PartId == 0 || _ids.VehId == 0 || _ids.RepId == 0)
        {
            Console.WriteLine("[!] Uwaga: Brak ID do usunięcia. Sprawdź obiekt '_ids'.");
            return;
        }

        Console.WriteLine(
            $">>> URUCHAMIAM: Test Usuwania (DELETE) dla ID: M:{_ids.MechId}, P:{_ids.PartId}, V:{_ids.VehId}, R:{_ids.RepId}");

        // Usuwanie Zlecenia Naprawy (Musi być jako pierwsze ze względu na klucze obce!)
        var dRep = await _client.DeleteAsync($"Repairment/api/delete/{_ids.RepId}");
        Console.WriteLine(dRep.IsSuccessStatusCode
            ? "[OK] Usunięto zlecenie naprawy"
            : "[!] Błąd usuwania zlecenia naprawy");

        // Usuwanie Pojazdu
        var dVeh = await _client.DeleteAsync($"Vehicle/api/delete/{_ids.VehId}");
        Console.WriteLine(dVeh.IsSuccessStatusCode ? "[OK] Usunięto pojazd" : "[!] Błąd usuwania pojazdu");

        // Usuwanie Mechanika
        var dMech = await _client.DeleteAsync($"Mechanic/api/delete/{_ids.MechId}");
        Console.WriteLine(dMech.IsSuccessStatusCode ? "[OK] Usunięto mechanika" : "[!] Błąd usuwania mechanika");

        // Usuwanie Części
        var dPart = await _client.DeleteAsync($"Parts/api/delete/{_ids.PartId}");
        Console.WriteLine(dPart.IsSuccessStatusCode ? "[OK] Usunięto część" : "[!] Błąd usuwania części");
    }

    // Klasa pomocnicza
    private class CreatedIds
    {
        public int MechId { get; set; }
        public int PartId { get; set; }
        public int VehId { get; set; }
        public int RepId { get; set; }
    }
}
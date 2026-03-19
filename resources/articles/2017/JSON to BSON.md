---
title: "JSON to BSON"
pubDate: 2017-04-30 21:36:00
categories:
  - C#
tags:
  - C#
  - .NET
  - MongoDB
  - NoSQL
  - XML
  - JSON
  - HTTP
  - Serialization
  - Generics
---

# JSON to BSON
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Sanıyorum her.Net programcısının takım çantasında yer alan paketlerden birisi de Newtonsoft'un JSON serileştirme kütüphanesidir. JSON (JavaScriptObjectNotation) formatı, XML (eXtensibleMarkupLanguage) şemasından sonra hafif ve az yer kaplama özellikleri nedeniyle çokça tercih edilen standartlardan birisi haline gelmiştir. Diğer yandan JSON içeriklerin Binary formatta serileştirilmiş versiyonu olarak adlandırılan BSON formatı da sıklıkla kullanılmaktadır.

[BSON](http://bsonspec.org/#/specification), NoSQL camiasının liderlerinden MongoDB ile popülerlik kazanmış bir veri formatı. JSON içeriğinin binary formatta sunulması oldukça hızlı gerçekleşebilecek bir işlem. Bu nedenle NoSQL tabanlı sistemlerde yatay ölçeklenen veri kümeleri arasındaki iletişimde tercih edilebiliyor. Ağ üzerinde dolaşan bu küçültülmüş paketler içerisinde JSON tipinde veri bulunuyor. Zaten tasarım amaçlarından birisi de bu.

Peki.Net tarafında kullanılan nesne örneklerini BSON formatına nasıl dönüştürebiliriz? Bunun bilinen pek çok yolu var tabii ki ancak uygulamanızda Newtonsoft ve JSON içerikleri ile çalışıyorsanız işi kolaylaştıracak tiplerde bu paketle birlikte geliyor. Newtonsoft.Json paketi, bir JSON içeriğinin binary olarak yazılması ve okunması için iki temel tip sunmakta. Bu tipleri kullanarak BSON dönüşüm işlemleri kolayca gerçekleştirilebilir. Basit bir örnekle konuyu özetleyelim.

> Başlamadan önce Newtonsoft'un ilgili paketinin uygulamaya yüklenmiş olması gerektiğini hatırlatalım. Bunu NuGet Package Manager veya Console üzerinden gerçekleştirebiliriz.
> ![image.axd](images/image.axd)

Aşağıdaki sınıf diagramında ve kod parçasında görülen tipleri tasarladığımızı düşünelim.

![image.axd](images/image.axd)

```csharp
public class Person
{
	public int PersonId { get; set; }
	public string Title { get; set; }
	public decimal Salary { get; set; }
	public Job[] Jobs { get; set; } // SOAPFormantter desteği için Array yapıldı

	public override string ToString()
	{
		return string.Format("{0}-{1}-{2}"
			, PersonId, Title, Salary.ToString("C2"));
	}
}

public class Job
{
	public int JobId { get; set; }
	public string Description { get; set; }
	public DateTime AddingTime { get; set; }
	public override string ToString()
	{
		return string.Format("\t{0}-{1}-{2}"
			, JobId, Description, AddingTime.ToShortDateString());
	}
}
```

Kişi bilgisini tuttuğumuz Person ve bu kişilerin önceden çalıştıkları işlerin bilgisini tutan Job isimli iki sınıfımız var. BSON dönüşümünde kullanılacak örneğin zengin olması için Person sınıfında Job tipinden bir dizi bulunuyor. Böylece bire-çok ilişki barındıran bir nesne ağacını dönüştürme işleminde kullanabiliriz. Tabii bize test verisi ve ters serileştirme sonrası dönüşen liste içeriğini ekrana basacak fonksiyonellikler de gerekiyor. Bunları şimdilik Utility isimli bir sınıfta aşağıdaki kod parçasında görüldüğü gibi toplayabiliriz.

```csharp
public class Utility
{
	public List<Person> FillEmployees()
	{
		List<Person> employees = new List<Person>();

		employees.Add(new Person
		{
			PersonId = 1,
			Title = "Coni Minomic",
			Salary = 1900,
			Jobs = (new List<Job>
			{
				new Job{ 
					JobId=1
					, Description="Google's Master Computer Scientist"
					, AddingTime= DateTime.Now
				},
				new Job{ 
					JobId=2
					, Description="Amazon Big Cloud System Manager"
					, AddingTime= DateTime.Now
				},
			}).ToArray()
		});

		employees.Add(new Person
		{
			PersonId = 1,
			Title = "Jon Carter",
			Salary = 1780,
			Jobs = (new List<Job>
			{
				new Job{ 
					JobId=1
					, Description="Microsoft Principle Developer"
					, AddingTime= DateTime.Now
				}
			}).ToArray()
		});

		return employees;
	}

	public void WriteLine(List<Person> personOfInterest)
	{
		foreach (var person in personOfInterest)
		{
			Console.WriteLine(person.ToString());
			foreach (var job in person.Jobs)
			{
				Console.WriteLine(job.ToString());
			}
		}
	}
}
```

FillEmployees metodu ile test amaçlı bir çalışan listesi oluşturuyoruz. WriteLine metodu ise gelen çalışan içeriğini ekrana bastırmakla yükümlü. Amacımız, çalışan listesini BSON formatında bir dosyaya yazdırmak ve sonrasında bu içeriği ters-serileştirme ile geri okuyup ekrana yazdırmak. Bunun için gerekli örnek kod parçasını aşağıdaki gibi geliştirebiliriz.

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace BsonExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var mario = new Utility();
            var employees = mario.FillEmployees();
            var root = Environment.CurrentDirectory;
            var file = Path.Combine(root, "employees.bson");
            JsonSerializer serializer = new JsonSerializer();

            using (MemoryStream mStream = new MemoryStream())
            {
                using (BsonWriter bsonWriter = new BsonWriter(mStream))
                {
                    serializer.Serialize(bsonWriter, employees);
                }
                File.WriteAllText(file, Convert.ToBase64String(mStream.ToArray()));
            }

            var fileContent=File.ReadAllText(file);
            var binaryContent=Convert.FromBase64String(fileContent);
            using(MemoryStream mStream=new MemoryStream(binaryContent)){
                using (BsonReader bsonReader = new BsonReader(mStream))
                {
                    bsonReader.ReadRootValueAsArray = true;
                    var result=serializer.Deserialize<List<Person>>(bsonReader);
                    mario.WriteLine(result);
                }
            }
        }
    }
	.
	.
	.
```

Koddaki başrol oyuncuları JsonSerializer, BsonWriter ve BsonReader sınıflarıdır. İlk using bloğunda MemoryStream ile belleğe açılan employees liste içeriğinin Base64 kodlamasından yararlanılarak dosyaya yazdırılması söz konusudur. İkinci using bloğunda ise bir kaç satır önce dosyaya yazılmış olan binary içeriğin Base64'ten byte[] haline getirilmesi ve ardından BsonReader kullanılarak generic Person listesine dönüştürülmesi işlemi yapılmaktadır. Kritik noktalardan birisi dePerson tipinden bir listenin (List) kullanılmasıdır. Eğer BsonReader nesne örneğinin ReadRootValueAsArray özelliğine true verilmezse bu listenin ters-serileştirilme aşamasında aşağıdaki ekran görüntüsünde yer alan JsonSerializationException oluşacaktır.

![image.axd](images/image.axd)

Kodun çalışması sırasında üretilen BSON içeriği ise aşağıdakine benzer olacaktır.

![image.axd](images/image.axd)

Yapılan ters serileştirme işlemi sonrası elde edilen ekran çıktısı da şöyledir.

![image.axd](images/image.axd)

Görüldüğü gibi BSON içeriği başarılı bir şekilde ters-serileştirilerek nesne haline getirilebilmiştir.

Örnekteki kod parçalarını biraz daha düzenlemeye çalışmanızı önerebilirim. Nitekim BSON dönüşüm operasyonları birer genişletme metodu (extension method) haline de getirilerek kullanımları daha kolay hele getirilebilir. Ayrıca Newtonsoft'un kütüphanelerine başvurmadan da JSON içeriği oluşturabilir ve Base64 kodlamasından yararlanarak BSON içerikleri üretebilirsiniz. Sonuçta JSON standartları belli. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

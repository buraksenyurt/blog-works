---
layout: post
title: "Basit Bir JsonConverter Uyarlaması"
date: 2018-09-22 11:28:00 +0300
description: "Elimizde aynı anlamları taşıyan ama key adları farklı olan JSON içerikleri bulunmakta. Farklı paketleri kullanan servislerden dönen farklı cevaplar oluşuyor. İstemci tarafı aynı anlamdaki bu farklı çıktılar için farklı sınıflar tasarlamak istemiyor. Acaba ters serileştirme işlemi sırasında araya girip farklı key değerlerini aynı nesne örneğinin özelliklerine atamamız mümkün olabilir mi? JsonConverter sınıfını kullanarak konuya çözüm üretmeye çalışıyoruz."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - json
  - bash
  - csharp
  - dotnet
  - web-api
  - serialization
  - reflection
  - generics
---
# Basit Bir JsonConverter Uyarlaması
![peter_1.jpg](/assets/images/2018/peter_1.jpg)

Merhaba Burak,

Nasılsın? Umarım iyisindir ve her şey yolundadır. Beni sorarsan her zaman ki gibi oldukça yoğun bir dönemden geçmekteyim. Özellikle halen devam etmekte olan mimari dönüşüm projesinden dolayı böyle bir yoğunluğumuz var. Gerçi çevikleşmeye başladığımızdan beri fazla mesai yapmıyor ve gerçekten değer içeren sürümlenebilir çıktılar üretiyoruz. Şu an altıncı sprint'i koşmaktayız ve takımın ivmesi rayına oturmuş durumda. Bu açılardan bakınca tatlı bir yoğunluktayım desem yeridir. Sana bunları anlatmama gerek yok nitekim duyduğuma göre siz de benzer bir sürece girmişssiniz.

Ben izninle kafama takılan bir konuyu seninle paylaşmak istiyorum. Firmamız bünyesinde geliştirdiğimiz Web API servislerinde standart input ve output tipleri kullanmaktayız. Payload'ları bunların içerisinde taşıyoruz. Bilirsin, POCO (Plain Old CLR Objects) şeklinde tasarladığımız klasik servis girdi çıktı mesajları işte. Lakin bu mesajların bazılarının farklı döndüğünü gördük. Aktif olarak Load Balancer'lar arkasında kalan servislerin bir kaçı aşağıdaki gibi bir JSON içeriği döndürmekteyken (basit anlatabilmek için içerikleri kırptım),

```json
{
	'operation_name':'Dosya transfer işlemi',
	'state':'Tamamlandı',
	'additional_info':'her şey yolunda',
	'time':'20180404195865'
}
```

bazıları da aşağıdaki gibi bir içerik döndürüyor.

```json
{
	'function_name':'Batch çalıştırma işlemi',
	'status':'hata aldı',
	'description':'batch yerinde bulunamamış',
	'time':'20180404190001'
}
```

Senin de göreceğin gibi servislerin bazıları output tipinin farklı bir versiyonunu kullanmakta. Bu sorunu çözmek üzere bir takım geliştirmelere başladık. Ancak bu sorun merak ettiğim başka bir konuyu daha doğurdu. Servisleri test ettiğim tarafta bu iki farklı çıktı için iki farklı sınıf yazmak zorunda kaldım. Bir şekilde mesajlar için araya girip JSON'ların key değerlerine göre tek bir nesne örneğine Deserialize işlemi uygulayabilir miyim? Uygulayabilirsek eğer bir örnek ile nasıl yapabileceğimizi bana anlatabilir misin? En kısa sürede görüşmek ümidiyle sevgili dostum. S (h) arp'a bol bol selamlar:)

### Sevgili Nazım,

Sendeki tatlı yoğunluğun bir benzeri bizde de var. Henüz ikinci sprint'teyiz. Taşlar yeni yeni oturmaya başladı. Sanırım bir sonraki sprint'te kapasitemiz ve ne kadarlık iş çıkartabildiğimize dair istatistiki değerler oturmaya başlayacak. Takım olarak hareket etmek oldukça güzel (benim gibi insanları sevmeye birisi için bile). Buradaki çevik takımların isimleri, karakterleri, logoları, duvar kağıtları da var. Oyunlaştırılmış bir deneyimi yaşadığımızı ifade edebilirim. Benim eşleştiğim karakter Ghostbusters'dan Peter Venkman:) Gelelim senin merak ettiğin problemin çözümüne...

Eğer istemci tarafında Newtonsoft'un güzide, meşhur, en mükemmel, en şık JSON paketini kullanıyorsan sanırım aşağıdaki örnek kod parçaslar işini görecektir diye düşünüyorum. Öncesinde tabii işin özetinden bahsedeyim. Deserialize işlemi sırasında araya girmek için JsonConverter sınıfından türetilmiş bir tipi ve içerisinde ezeceğimiz (override) ReadJson metodunu kullanabiliriz. Aslında özelleştirilmiş bir ters serileştirme işlemi söz konusu.

Bu senaryoda aynı anlamı taşıyan ama farklı isimlendirilmiş JSON içerikleri var. key:value çiftlerindeki key bilgileri için basit bir eşleşme tutmamız ve value değerlerine göre kendi tarafımızdaki.Net nesnesini JsonProperty nitelikleri ile desteklememiz yeterli görünüyor. Internet'ten yaptığım araştırmalar sonucunda örnek bazı kod parçalarını rastladım. Reflection ile az da olsa haşır neşir olmamız gerekiyor ki bu kötü bir şey değil. Dün gece de West-World'ün başına geçtim ve basit bir.Net Core Console projesi açtım. İlk iş Newtonsoft.Json paketini projeye dahil ettim. Aşağıdaki gibi.

```bash
dotnet add package Newtonsoft.Json --version 11.0.2
```

Sonrasında ServiceResponseConverter isimli şu sınıfı geliştirmeye başladım.

```csharp
public class ServiceResponseConverter
	: JsonConverter
{
	private readonly Dictionary<string, string> mappings = new Dictionary<string, string>
	{
		{"operation_name", "function_name"},
		{"state", "status"},
		{"additional_info", "description"},
		{"time","time"}
	};

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var instance = Activator.CreateInstance(objectType);
		var properties = objectType.GetTypeInfo().DeclaredProperties.ToList();

		var payload = JObject.Load(reader);
		foreach (var property in payload.Properties())
		{
			if (!mappings.TryGetValue(property.Name, out var name))
				name = property.Name;

			var instanceProperty = properties.FirstOrDefault(p => p.CanWrite && p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == name);
			instanceProperty?.SetValue(instance, property.Value.ToObject(instanceProperty.PropertyType, serializer));
		}

		return instance;
	}

	public override bool CanWrite => false;

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}
	public override bool CanConvert(Type objectType)
	{
		return objectType.GetTypeInfo().IsClass;
	}
}
```

Gördüğün üzere senin örnek JSON içeriklerindeki key değerlerinin eşleştirildiği mappings isimli bir generic Dictionary koleksiyonumuz var. Serialization işlemi uygulamayacağımız için WriteJson fonksiyonunu ele almadım. Bunun yerine ReadJson fonksiyonuna odaklanmanda yarar var. Öncelikle ters serileşmede ki hedef nesne örneğini üretmekteyiz. Hangi tipten üreteceğimiz bilgisi objectType değişkeninde geliyor. JSON içeriğinden okuma yapıp özelliklere değer atayacağımızdan properties isimli bir liste de oluşturuyoruz. Ardından reader içeriğini okuyup JObject türünden olan payload nesnesine alıyoruz. Derken payload içerisindeki tüm özellikleri dolaşmaya başlıyoruz. mappings'deki eşletirmeleri ve hedef sınıfın nitelik tanımlarındaki isimleri kullanarak bir atama gerçekleştiriyoruz. Dibine kadar reflection kullandığımızı fark etmiş olmalısın. Eski C# bilgilerini hatırlamanın zamanı gelmişti zaten;) Deserialize işlemine konu olacak hedef sınıf içeriği ise aşağıdaki gibi.

```csharp
[JsonConverter(typeof(ServiceResponseConverter))]
public class ServiceResponse
{
	[JsonProperty("function_name")]
	public string Operation { get; set; }
	[JsonProperty("status")]
	public string State { get; set; }
	[JsonProperty("description")]
	public string Info { get; set; }
	[JsonProperty("time")]
	public string ProcessingTime { get; set; }

	public override string ToString()
	{
		return $"Operation : {Operation}\nState : {State}\nInfo : {Info}\nProcessing Time : {ProcessingTime}\n";
	}
}
```

Dikkat edersen JsonProperty niteliklerinde mappings listesindeki value adları kullanılmış durumda. Ayrıca ServiceResponse sınıfına uygulanan JsonConverter niteliğine parametre olarak ServiceResponseConverter tipini vermeliyiz ki Deserialize işleminde devreye girebilsin. Örnek main metodu kodlarımız da aşağıdaki gibi.

```csharp
static void Main(string[] args)
{
	string sample_1 = @"
					{
						'operation_name':'Dosya transfer işlemi',
						'state':'Tamamlandı',
						'additional_info':'her şey yolunda',
						'time':'20180404195865'
					}";
	string sample_2 = @"
					{
						'function_name':'Batch çalıştırma işlemi',
						'status':'hata aldı',
						'description':'batch yerinde bulunamamış',
						'time':'20180404190001'
					}";

	Convert(sample_1);
	Convert(sample_2);
}

static void Convert(string payload)
{
	var result = JsonConvert.DeserializeObject<ServiceResponse>(payload);
	Console.WriteLine(result.ToString());
}
```

JsonConvert sınıfının DeserializeObject fonksiyonuna generic ServiceResponse bildirimi yapılmış durumda. Çalışma zamanında bu sınıfa uygulanan nitelik sebebiyle özelleştirdiğimiz ters serileştirme akışı devreye girecektir. Senin verdiğin mesaj içeriklerini kullanarak yazdığım basit bir test kodu. Elde ettiğim sonuçlar aşağıdaki ekran görüntüsündeki gibiydi.

![jsonconverter_1.gif](/assets/images/2018/jsonconverter_1.gif)

Sanıyorum ki senin istediğin de buna benzer bir şeylerdi. Elbette daha iyi şekilde geliştirilebilir bu kod parçası. Hatta hem sayı hem de isimlendirme olarak birbirlerinden tamamen farklı mesajları tek bir tip içerisinde toplamaya çalışmayı da deneyebilirsin. Umarım az da olsa fikir sahibi olmuşssundur. Ben epey şey öğrendim diyebilirim. En kısa sürede görüşmek dileğiyle. Ufaklığa bol bol selamlar:)
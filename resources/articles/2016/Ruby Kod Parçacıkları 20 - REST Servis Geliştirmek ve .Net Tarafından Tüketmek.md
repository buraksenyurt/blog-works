---
title: "Ruby Kod Parçacıkları 20 - REST Servis Geliştirmek ve .Net Tarafından Tüketmek"
pubDate: 2016-02-09 01:44:00
categories:
  - REST
  - Ruby
tags:
  - REST
  - Ruby
  - Bash
  - C#
  - .NET
  - JSON
  - HTTP
  - Authentication
  - Authorization
  - Serialization
  - Generics
---

# Ruby Kod Parçacıkları 20 - REST Servis Geliştirmek ve .Net Tarafından Tüketmek
![logo.png](images/logo.png)

Merhaba Arkadaşlar,

Artık belli bir platforma bağlı kalmadan farklı uygulamaları bir araya getirip konuşturabiliyor, büyük ölçekli sistemleri uçtan uca bağlayarak süreçler işletebiliyoruz. Burada programlama dillerinin üzerine oturduğu çatıların (Frameworks) büyük avantajlar sağladığı ve işleri belirli ölçüde kolaylaştırdığı aşikar.

Elbette işin en önemli kısmı yine de servislere düşüyor. Çözümler için düşünülen mimariler mutlak suretle servisleri ele alıyor. Hali hazırda SOA (Service Oriented Architecture) üzerine kurulu sayısız çözüm mevcut. Yeni geliştirilen kurumsal çözümler için de SOA mutlaka göz önüne alınıyor. Son yıllarda Micro Service yaklaşımı ile hafif siklet servislerin süreçlere dahil edilmesi de oldukça popüler. Herhangi bir programlama dili ile Lightweight servisler geliştirmek, bunları ucuz sunucular üzerinde organize ederek kolayca dağıtılabilir ve ölçeklenebilir şekilde sunmak oldukça pratik.

İşte bu düşünceler ışığında camdan dışarı bakarken gönlümden Ruby tarafında bir servis geliştirmek ve bunu bir.Net uygulamasında kullanmak geçiyordu. Ruby tarafında hafif siklet ve REST-Representational State Transfer modelinde bir servis geliştirmek işin benim için en önemli kısmıydı..Net Client'ı yazmak nispeten daha kolaydı. Ve sonuç olarak gönlümdeki düşünceleri işte bu yazı ile kaleme almaya karar verdim.

## Senaryo

Öncelikli olarak senaryomuza bir bakalım. Kabaca aşağıdaki şekilde görülen enstrümanları kullanarak bir çözüm geliştirmeye çalışcağız.

![rest_6_n.gif](images/rest_6_n.gif)

> Senaryo: Ruby programlama dilini kullanarak basit bir REST servis geliştireceğiz. Bu servis SQLite üzerinde duran AdventureWorks isimli veritabanındaki Product tablosunu sorgulayabilmemiz için iki operasyon sunacak. HTTP Get ile çalıştıracağımız taleplere ait sonuçları istemci tarafına JSON formatında döndürmeyi planlıyoruz. İstemci tarafı basit bir tarayıcı olabileceği gibi,.Net veya herhangi platformda yazılmış bir uygulama da olabilir. İstemci tarafında REST taleplerini kolaylaştırmak içinse RestSharp paketinden yararlanacağız.

## Bize Gerekenler

Veriyi tutmak için SQLite veritabanından yararlanacağız. Bu veritabanının basit kullanımı için [bir önceki yazımıza](https://www.buraksenyurt.com/post/ruby-kod-parcaciklari-19-sqlite-ile-basit-veritabani-islemleri.aspx) bakabilirsiniz. Ruby tarafında REST servislerini kolayca geliştirmek için Sinatra çatısından yararlanacağız. Kullanımı son derece kolay olan Sinatra için

gem install sinatra

ifadesini Ruby komut satırından çalıştırmamız yeterli..Net tarafındaki istemcide REST taleplerini oluşturabilmek ve gelen içeriği JSON'dan nesnelleştirmek için RestSharp paketinden faydalanacağız.

## Servis Tarafı Kodu

SQLite ve Sinatra ile ilgili hazırlıklarımız tamamsa, aşağıdaki kod dosyasını yazarak servis tarafını ayağa kaldırmaya başlayabiliriz.

```bash
# Simple REST service
require 'sinatra'
require 'json'
require 'sqlite3'

#list all products
get '/products' do
	begin
		products=Array.new
		db=SQLite3::Database.open "AdventureWorks.db"
		db.results_as_hash=true;	 
		rows=db.execute "Select product_id,title,list_price from Product"
		rows.each do |row|
			products << Product.new(row['product_id'],row['title'],row['list_price'])
		end
		products.to_json
	rescue SQLite3::Exception => excp
		excp
	ensure
		db.close if db
	end
end

#get specific product
get '/products/:id' do
	begin
		product=nil
		db=SQLite3::Database.open "AdventureWorks.db"
		db.results_as_hash=true;	 
		row=db.get_first_row("Select product_id,title,list_price from Product where product_id=?",params[:id])
		if row!=nil
			product=Product.new(row['product_id'],row['title'],row['list_price'])	
			product.to_json
		else
			"Not Found"
		end
	rescue SQLite3::Exception => excp
		excp
	ensure
		db.close if db
	end
end
	
class Product
	attr_accessor:id
	attr_accessor:name
	attr_accessor:list_price
	
	def initialize(id,name,list_price)
		@id=id
		@name=name
		@list_price=list_price
	end	
	def to_json(*a)
		{
			"json_class" => self.class.name,
			"data"       => {"id" => @id, "name" => @name, "list_price" => @list_price}
		}.to_json(*a)
	end 
	def self.json_create(object) 
		new(object["data"]["id"], object["data"]["name"],object["data"]["list_price"])
 	end
	
	def to_s
		"#{@id}-#{@name}-#{@list_price}"
	end
end
```

Geliştirdiğimiz servis REST tabanlıdır. Dolayısıyla HTTP Get, Post, Put, Delete gibi taleplere cevap verecek şekilde çalışır. Sinatra altyapısı sayesinde bu tip istekleri fonksiyonelleştirmek oldukça kolaydır.

get 'products'do

end

ve

get '/products/:id'do

end

metodları ile http://adres/products ve http://adres/products/id adreslerine gelen talepleri karşılayabiliriz. İlk metod ile Product tablosundaki tüm ürün listesinin JSON formatında sunulması söz konusudur. Diğer metod:id ile gelen değere göre (ki Product tablosundaki productid alanının içeriği oluyor) yapılan arama sonucunu yine JSON formatında döndürmektedir.

SQLite üzerinden gerçekleştirilen select sorguları sonucu elde edilen içerikleri Product sınıfına ait nesne örneklerine aldığımıza dikkat edelim. Product sınıfında JSON serileştirme işlerini gerçekleştirmek adına bazı düzenlemeler olduğu da gözden kaçmamalıdır (JSON serileştirme ile ilgili olarak [buradaki yazıyı](https://www.buraksenyurt.com/post/ruby-kod-parcaciklari-16-json-serilestirme.aspx) referans alabilirsiniz)

## Çalıştırma ve İlk Testler

Yazdığımız Ruby kod dosyasını çalıştırdığımızda otomatik olarak localhost:4567 adresi üzerinden servisimizin yayına başladığını görebiliriz.

![rest_4.gif](images/rest_4.gif)

4567 Sinatra'nın varsayılan olarak kullandığı Port bilgisidir. İsterseniz değiştirebilirsiniz ancak nasıl yapıldığını söylemek istemiyorum:) Lütfen araştırın.

Bu işlemin ardından basit bir tarayıcı yardımıyla Get operasyonlarını deneyebiliriz.

http://localhost:4567/products sonucu

![rest_1.gif](images/rest_1.gif)

Görüldüğü gibi bir önceki yazımızda Product tablosuna eklediğimiz satırların tamamına ulaşabildik.

http://localhost:4567/products/1 sonucu

![rest_2.gif](images/rest_2.gif)

Bu kez productid değeri 1 olan ürünü elde etmeyi başardık. Pek tabii var olmayan bir ürünü girersek Not Found çıktısını almamız gerekiyor.

http://localhost:4567/products/10 sonucu

![rest_3.gif](images/rest_3.gif)

Doğal olarak bulunamayan içerikler için çok daha şık bir sayfayı istemciye sunabiliriz.

Gerçekleştirilen bu çağrılar aynı zamanda komut satırına da yansır. Aşağıdaki ekran görüntüsünde dikkat edileceği üzere servise gelen tüm çağrılar loglanmıştır.

![rest_5.gif](images/rest_5.gif)

## .Net Client

Servis hazır olduğuna göre bunu tüketecek bir istemci uygulama yazmayı da deneyebiliriz. Ben daha çok aşina olduğum.Net platformunu tercih ediyorum. Basit bir Console uygulaması işimizi görür. Host ettiğimiz servis REST tabanlı olduğu için talepleri (Request) gönderme ve gelen JSON içeriğini ters serileştirme (Deserialization) işlemlerinde RestSharp paketini kullanabiliriz.

![rest_7.gif](images/rest_7.gif)

Paketi Console uygulamasına entegre ettikten sonra aşağıdaki kod içeriğini yazarak ilerleyebiliriz.

```csharp
using RestSharp;
using RestSharp.Deserializers;
using System;

namespace AW.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonDeserializer serializer = new JsonDeserializer();
            var client = new RestClient("http://localhost:4567");
            var request = new RestRequest("products", Method.GET);
            RestResponse response = (RestResponse)client.Execute(request);
            Console.WriteLine(response.Content);

            var request2 = new RestRequest("products/1", Method.GET);
            var response2=client.Execute(request2);

            var result=serializer.Deserialize<SingleRoot>(response2);
            Console.WriteLine(result.Product.ToString());
        }
    }
    
    [DeserializeAs(Name="json_clas")]
    class SingleRoot
    {
        [DeserializeAs(Name="data")]
        public Product Product { get; set; }
    }
    class Product
    {
        public int id { get; set; }
        public string name {get; set; }
        public int list_price { get; set; }
        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", id, name, list_price);
        }
    }
}
```

RestSharp kütüphanesinin kullanımı oldukça basittir. Öncelikle RestClient tipinden bir nesne örneği oluşturulur ve HTTP talep adresleri için kullanılacak kök adres belirtilir. Örneğimizde host ettiğimiz kök adres http://localhost:4567 şeklindedir. products ve products/:id talepleri için RestRequest tipinden yararlanılır. Bu sınıfın yapıcı metoduna (Constructor) gelen ilk parametre ile HTTP adresi, ikinci parametre ile de HTTP metodunun tipi belirtilir. Örneklerimizde Get metodunu kullandığımız için Method.GET değeri verilmiştir. RestResponse nesne örneğine dolacak olan JSON içeriklerine Content özelliği üzerinden doğrudan erişilebileceği gibi JsonDeserializer sınıfının generic Deserialize metodu kullanılarak, içeriğin ters serileştirilmesi ve nesnel olarak ele alınması da sağlanabilir.

Ruby tarafındaki servisin verdiği JSON içeriğinde jsonclass ve data isimli iki özellik yer almaktadır. Bu nedenle.Net tarafındaki sınıflar söz konusu şema yapısına göre tasarlanmış ve isimlendirme uyumluluğu sorununu çözmek için DeserializeAs niteliğinden (Attribute) yararlanılmıştır. Nitekim istemci tarafındaki sistem içerisinde sorgu sonuçlarını json_class ve data isimli olacak şekilde dolaştırmak çok doğru değildir.

İlk talep ile tüm ürünlerin listesi elde edilir. Bu kısımda size düşen görev JSON içeriğini Product tipinden bir koleksiyon ile (List türevli bir nesne örneğinde) nasıl karşılayabileceğimizdir. Servis çalıştığı sürece.Net istemcisi bize aşağıdaki sonuçları döndürür.

![rest_8.gif](images/rest_8.gif)

## Sizin Yapabilecekleriniz

Servis şu anda ayakta. Ancak bazı eksiklikleri var. Örneğin Post ve Delete gibi metodları uygulamadık. Açıkçası servisten yararlanarak Product tablosuna veri ekleyebilsek veya silebilsek hiç fena olmaz. Hatta istemci tarafını bir Web uygulaması olarak tasarlayıp görsel açıdan da zenginleştirerek veri odaklı bir çözüm üretebilir ve CRUD operasyonlarını bu şekilde sunabiliriz. Bu kutsal görevi siz değerli okurlarıma bırakıyorum.

## Değerlendirme

Geliştirmek istediğimiz senaryoyu biraz daha geniş çaplı düşünelim. Bulunduğumuz kurumda Ruby ve Ruby on Rails ile geliştirme yapan bir takım olduğunu düşünelim. Bu takımın şirket içerisinde geliştirilen bir başka ürüne sağlaması gereken hizmetler olduğunu varsayalım. Bu durumda ilgili ekipten bu fonksiyonellikleri sunacak servisleri hakim oldukları ortamda geliştirmek üzere bir talepte bulunabiliriz. Geliştirilen servis bir sunucuya dağıtıldıktan sonra kurum için herhangi bir uç nokta tarafından kolayca erişilip kullanılabilir. Servis tarafında Authentication/Authorization işlemleri veya güvenlik ile ilgili tedbirler alınarak kurum prosedürlerine uygun hale getirilmesi de sağlanabilir.

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

---
title: "Python Öğrenmeye Devam - Sınıflar"
pubDate: 2016-09-01 01:00:00
categories:
  - python
tags:
  - python
  - .NET
  - Entity Framework
  - Java
  - Ruby
---

# Python Öğrenmeye Devam - Sınıflar
![practice-makes-perfect.jpg](images/practice-makes-perfect.jpg)

Merhaba Arkadaşlar,

Bir süredir izinde olduğum için yeni gözdem Python ile yeterince ilgilenemedim. Döndüğümde daha önceden çalıştığım konuları hatırlamakta da epeyce zorlandım. Pasımı atmak ve tekrardan verimliliğimi yükseltmek için ilk olarak defterime karaladığım notlarımın üstünden geçtim.

Sonrasında ise C# dili ile geçimini sağlayan birisi olduğumdan Pyhton'un Nesne Yönelimli Dünyadaki yerini öğrenmeye çalışırsam bazı şeyleri daha kolay anlarım diye düşündüm. Derken kendimi nesne yönelimli dillerin en temel yapıtaşlarından birisi olan sınıfların Python içerisindeki yerini araştırırken buldum. İşte bir.Net geliştiricisi olarak edindiğim bilgiler (Eğer.Net veya Java dünyasından geliyorsanız temel seviyede de olsa nesne yönelimli programlama kavramlarını biliyorsunuzdur. Yazıyı ben de kendime göre hazırladığımdam sizin de bildiğinizi varsayarak ilerleyeceğim)

Ne tesadüftür ki Python dilinde de aynen Ruby dilinde olduğu gibi herşey bir nesne olarak ele alınmaktadır. Python, OOP'un pek çok yeteneğine sahip bir dil olarak kullanılabilir. Örneğin Inheritance (hatta Ruby'nin doğrudan desteklemediği çoklu kalıtım-MultiInheritance) desteklenir. Tam olarak OOP'un temek kavramları diyemesek de Overriding, Name Hiding gibi.Net veya Java dünyasının bilinen unsurları da konusudur. Tabii bakış açımızı biraz değiştirmemiz de gerekebilir. Nitekim Python,.Net veya Java tarafındaki gibi derlemeli değil yorumlamalı bir dildir. Yani bir sınıfı çalışma zamanında (runtime) tasarlayıp yorumlattıktan sonra, yeni kabiliyetler kazandırabilir ve niteliklerini çalışma zamanının ilerleyen bölümlerinde değiştirebiliriz.

## İlk Sınıfımız

Pek tabii OOP denilince dilin bu amaçla desteklediği en temel tip (type) sınıftır. Şimdi basit bir sınıf tasarımı ile işe başlasak fena olmaz diyorum.

```text
class Product:
	def __init__(self,title,price,stockLevel):
		self.title=title
		self.price=price
		self.stockLevel=stockLevel
		
	def writeToScreen(self):
		info="{0} {1} {2}"
		print(info.format(self.title,self.price,self.stockLevel))
		
	def discount(self,value):
		self.price=self.price-value
		
meks=Product("Mexican",160,5)
meks.writeToScreen()
meks.discount(16)
meks.writeToScreen()
print(meks.title)
print(meks.price)
print(meks.stockLevel)
meks.price=19
print(meks.price)
```

![pyhtonc_1n.gif](images/pyhtonc_1n.gif)

(Kodu [Repl.it](https://repl.it) üzerinden denediğimi belirteyim. Malum şirketin kısıtlı imkanları ve kuralları nedeniyle direkt bilgisayar üzerinden çalışamıyorum)

Klasik olarak kobay sınıflarımızdan birisini ele aldık. Product sınıfı içerisinde 3 adet fonksiyon bulunuyor. init isimli metod aslında.Net dünyasından aşina olduğumuz yapıcı fonksiyon (Constructor) gibi düşünülebilir. Product nesnesi örneklenirken (ki meks isimli değişkenimiz bir Product nesne örneğidir) örnek niteliklerine (instance attribute) ilk değerlerini atıyoruz. Burada bir.Netçi olarak kafam biraz karıştı aslında. Nitekim C# kodu yazar gibi düşünüp ortalarda bir yerlerde property veya field tanımlaması gibi bir şeyler aradım. Aslında bu mümkün. Sınıf içerisinde nitelikleri açık bir şekilde tanımlayabiliyoruz ancak bunlar sınıf niteliği (Class Attribute) olarak anlam kazanıyorlar.

Bizim örneğimizdeki nitelikler ise nesne örneği niteliği olarak değer bulmakta. Nesne örneği nitelikleri adından da anlaşılacağı üzere kullanılabilmesi ve değer atanabilmesi için sınıfa ait bir nesne örneğine (nesne referansına) ihtiyaç duymakta. Tam tersine sınıf nitelikleri ise nesne örneği olmadan doğrudan sınıf adı üzerinden erişilebilen alanlar olarak kullanılmakta (Dilim varmıyor ama sanki static alan veya özellikler gibiler. Dilim varmıyor diyorum çünkü arka planda bu niteliklerin nasıl konuşlandırıldığını henüz öğrenmiş değilim)

Örnek kod parçasında kullanılan nesne örneği niteliklerine init fonksiyonu tarafından ilk değerleri atanmıştır. Kodun ilerleyen kısmında oluşturulan nesne örnekleri üzerinden bu niteliklere erişilebilir ve hatta sahip oldukları değerler değiştirilebilir.

init metodunda ve aslında sınıfa ait diğer fonksiyonlarda dikkat çeken en önemli nokta ise self anahtar kelimesinin kullanımıdır. self aslında çalışma zamanında oluşturulan sınıfa ait referansı (nesne örneğini) temsil etmektedir. Bu sayede fonksiyonlardan çalışma zamanındaki nesne örneğine ait niteliklere erişebilmek mümkündür. Yani nesne örneği üzerinden çağırılan metodlar aslında ait oldukları referansı self anahtar kelimesi ile kullanabilmektedir. self anahtar kelimesi bazı katı kuralları da beraberinde getirmekte. Örneğin metodun ilk sırasında yer alması ve adece fonksiyonlar içerisinde kullanılabiliyor olması gibi kurallar söz konusu (self anahtar kelimesini açıkçası C# tarafındaki this anahtar kelimesine benzettiğimi ifade edebilirim)

## self, Sınıf ve Nesne Örneği Nitelikleri Arasındaki İlişki

Aslında hazır yeri gelmişken self, sınıf ve nesne örneği nitelikleri arasındaki ilişkiyi örnek bir kod parçası ile incelemeye çalışalım.

```text
class Product:
	
	isActive=True
	
	def __init__(self,title,price,stockLevel):
		self.title=title
		self.price=price
		self.stockLevel=stockLevel
		self.isActive=False
		
meks=Product("Mexican",160,5)
print(meks.isActive)
print(Product.isActive)
```

![pyhtonc_2.gif](images/pyhtonc_2.gif)

Kodun çalışma zamanı çıktısına bakıldığında isActive için önce False sonrasında ise True yazıldığı görülmektedir. meks isimli nesne örneğini oluşturduğumuzda çalışma zamanı öncelikle init içerisinde isActive isimli bir nesne örneği niteliği kullanılıp kullanılmadığına bakar. Kullanıldığı için meks.isActive çıktısı False olarak ekrana yansımıştır (Burada nitelik bazında bir isim gizleme-Name Hiding- olduğunu düşünebiliriz sanırım) Ancak bir sonraki satırda çok daha farklı bir durum söz konusudur. isActive isimli sınıf niteliğine (3ncü satır oluyor) erişmek için Product.isActive ifadesi kullanılmıştır. Bu durumda çalışma zamanı Product sınıfına ait bir niteliğe bakması gerektiğini anlamıştır. Tahmin edileceği üzere aslında sınıf nitelikleri aynı kapsam içerisinde yer alan tüm ürünler için ortak bir özellikmiş gibi düşünülebilir.

Ben her ne kadar sınıfları incelemeye çalışsam da özellikle sınıf ve nesne örneği niteliklerinin çok çok önemli olduklarını da gördüm. Sınıf niteliklerinin tüm nesne örnekleri için ortak olmasının garip (veya deneyimli bir yazılımcı için tahmin edilebilir) sonuçları var. Söz konusu durumu anlamak için aşağıdaki kod parçasını ile devam edelim.

```text
class GameZone:
	
	players=[]
	
	def __init__(self,name):
		self.name=name
	
red=GameZone("Red Zone I")
red.players.append("burki")
print("Red Players ",red.players)

blue=GameZone("Blue One")
print("Blue players ",blue.players)
```

![pyhtonc_4.gif](images/pyhtonc_4.gif)

GameZone sınıfı içinde players isimli bir nitelik yer almaktadır. Bu niteliğe red ve blue isimli sınıf örnekleri üzerinde erişebiliyoruz. Dikkat edilmesi gereken nokta blue isimli GameZone nesnesi örneklendikten sonra yine bu nesne örneği üzerinden erişilen players niteliğinden burki sonucunun dönmesidir. Yani red nesnesi örneklendikten sonra players niteliğine eklenen değer, yeni örneklenen blue nesnesi için de söz konusudur. İşte nesne örneği niteliklerini kullanmanın bir sebebi de bu durumun ortadan kaldırılmasıdır (Tabii gerekiyorsa) Aynı kodu nesne örneği niteliği ile denersek sonuçlar daha farklı olacaktır.

```text
class GameZone:
	
	def __init__(self,name):
		self.players=[]
		self.name=name
		
	
red=GameZone("Red Zone I")
red.players.append("burki")
print("Red Players ",red.players)

blue=GameZone("Blue One")
print("Blue players ",blue.players)
```

![pyhtonc_5.gif](images/pyhtonc_5.gif)

Görüldüğü gibi players niteliği nesne örneğine özel hale getirildi.

## self Bir Anahtar Kelime midir?

> Şimdi sıkı durun. Enteresan bir kural geliyor. Bir sınıf içerisinde tanımlı metodların ilk parametresi aslında self anahtar kelimesi olarak değerlendirilir. Yani self aslında bir anahtar kelime olarak düşünülmeyebilir.

Bu durumu anlamak için Product sınıfına ait kod içeriğini aşağıdaki gibi değiştirerek ilerleyelim.

```text
class Product:
	def __init__(this,title,price,stockLevel):
		this.title=title
		this.price=price
		this.stockLevel=stockLevel
		
	def writeToScreen(this):
		info="{0} {1} {2}"
		print(info.format(this.title,this.price,this.stockLevel))
		
	def discount(this,value):
		this.price=this.price-value

meks=Product("Mexican",160,5)
meks.writeToScreen()
meks.discount(16)
meks.writeToScreen()
print(meks.title)
print(meks.price)
print(meks.stockLevel)
meks.price=19
print(meks.price)
```

![pyhtonc_3.gif](images/pyhtonc_3.gif)

Dikkat edileceği üzere self anahtar kelimesi yerine this kullanılmıştır. Python'un sıkı kuralları gereği this nesne örneği niteliklerine ulaşılabilmesi için yeterlidir. Ne var ki Python topluluğuna göre self anahtar kelime haline gelmiş bir söcüktür. Yani Python ile kod yazan birisinin self kelimesini kullanması önerilmektedir.

## .Netçi İçin Daha Tanıdık Bir Örnek

Dilerseniz ORM araçları kullanan bizler için bir örnek geliştirerek yazımızı yavaş yavaş sonlandıralım.

```text
class Category:
	
	def __init__(self,name,id):
		self.name=name
		self.id=id
		
class Product:
	
	def __init__(self,title,listPrice,category):
		self.title=title
		self.listPrice=listPrice
		self.Category=category
		
	def writeTo(self):
		info="\'{}\' ({} TL) from {}"
		print(info.format(self.title,self.listPrice,self.Category.name))

books=Category("Computer Books",1)

pythonBook=Product("Programming with Python",45,books)
pythonBook.writeTo()

cBook=Product("C for Dummies",24.50,books)
cBook.writeTo()
```

Örnek kodumuzda Category ve Product isimli iki sınıf yer almaktadır. Tahmin edeceğiniz üzere bir Product nesnesinin bir Category nesnesi ile ilişkilendirilmesi söz konusudur. Bunu yapmak oldukça basittir. Product sınıfına ait init metodunda yer alan self.Category=category ataması bu bağlantının kurulması için yeterli olmuştur. Böylece çalışma zamanındaki pythonBook ve cBook isimli Product nesne örnekleri, books isimli Category nesne örneği ile ilişkilendirilmiştir. Bir nevi nesneler arası bire-çok ilişki tanımladığımızı ifade edebiliriz. Çalışma zamanı çıktısı aşağıdaki gibidir.

![pyhtonc_6.gif](images/pyhtonc_6.gif)

Şimdi bu örnekten hareket ederek aslında Entity Framework içerisindeki Code First yaklaşımını çok basit düzeyde inşa etmeye çalışabiliriz. Bir kaç Entity sınıfı, bunlar arası ilişkilerin kurulması ve bir Context tipi başlangıç için yeterli olabilir. Aynen aşağıdaki kod parçasında olduğu gibi.

```text
class AzonContext:
	
	def __init__(self):
		self.Categories=[]
		self.Products=[]
	
class Category:
	
	def __init__(self,name,id):
		self.name=name
		self.id=id
		
class Product:
	
	def __init__(self,title,listPrice,category):
		self.title=title
		self.listPrice=listPrice
		self.Category=category
		
	def writeTo(self):
		info="\'{}\' ({} TL) from {}"
		print(info.format(self.title,self.listPrice,self.Category.name))

context=AzonContext()

books=Category("Computer Books",1)
context.Categories.append(books)

pythonBook=Product("Programming with Python",45,books)
cBook=Product("C for Dummies",24.50,books)
context.Products.append(pythonBook)
context.Products.append(cBook)
		
for b in context.Products:
	b.writeTo()
```

Dikkat edileceği üzere AzonContext sınıfına ait init metodu içerisinde Categories ve Products isimli listeler tanımlanmıştır. Listeler nesne örneği niteliği olduğundan context değişkeni üzerinden erişilip kullanılabilir. Bu şekilde kitaplar ve kategoriler ilgili listelere eklenebilir. Son satırda yer alan for döngüsü ile de eklemiş olduğumuz kitaplar ve bu kitaplara ait bilgiler ile dahil oldukları kategori verisi ekrana yazdırılmıştır.

![pyhtonc_7.gif](images/pyhtonc_7.gif)

Bu yazımızda Python dilinde bir sınıfın nasıl tasarlanabileceğini incelemeye çalıştık. Bunu yaparken sınıf ve nesne örneği niteliklerine, init operasyonuna, self kullanımına bakmaya çalıştık. Code First yaklaşımındaki sınıf tanımlamaları ve ilişkileri en basit haliyle inşa ettik. Python için nesne yönelimli kavramlar bu yazıya sığmayacak kadar fazla elbette. Örneğin nasıl ele alındığını incelemem gerekiyor. Hatta çoklu kalıtımın (Multi-Inheritance) uygulamasına da bakmalıyım. Diğer yandan bu yazıda değindiğiniz sınıf ve nesne örneği nitelikleri dışında sınıf ve nesne örneği metodları da (aslında self içeren metodlarımız nesne örneği fonkisyonlarıdır) var. @classmethod gibi bir decorator kavramı var ki bu sayede bir metodun sınıf metodu olması sağlanabiliyor. Yani anlayacağınız bakmam gereken bir çok şey var. Bu ve benzeri diğer konuları ilerleyen yazılarımızda ele almaya çalışacağım. Şimdilik öğrendiklerim bunlarlar sınırlı diyebilirim. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

---
title: "Ruby Kod Parçacıkları 23 - set,prime,logger,pathname,benchmark,find,zlib Kullanımları"
pubDate: 2016-05-31 21:00:00
categories:
  - Ruby
tags:
  - Ruby
  - Bash
  - HTTP
---

# Ruby Kod Parçacıkları 23 - set,prime,logger,pathname,benchmark,find,zlib Kullanımları
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Epey zamandır Ruby notlarımın (özellikle de Amazon'dan getirttiğim kitapların) yüzüne bakmadığımı gördüm de...Pişmanlık hissettim (İtiraf ediyorum bu aralar biraz tembellik yapıyordum) Sonra onunla uğraşmayı özlediğimi fark ettim. Onun sadeliğini, yalın yazımını, gücünü, pratikliğini. Pasımı atmak için bir şeyler araştırmam gerekiyordır. Derken kendimi her zaman ki gibi Google'da [Ruby standart kütüphanesi](http://ruby-doc.org/stdlib-2.3.1/) ile ilgili faydalı kod parçalarını ararken buldum.

Malumunuz kodlama yaparken hayatımızı kolaylaştıran pek çok fonskiyonellik, kullandığımız dillere ait standart kütüphaneler içerisinde gelmektedir. Genel ve temel ihtiyaçları barındıran bu kütüphanalerin bir benzeri de doğal olarak Ruby programlama dili için söz konusu. Standart kütüphane içerisinde işlerimizi kolaylaştıracak yetenekli pek çok tip ve fonksiyon yer almakta. Ben makalenin ilerleyen bölümlerinde geçtiğimiz hafta fırsat buldukça incelediğim bir kaç tanesine yer veriyorum.

## Belki de tekil bir veri yapısına ihtiyacımız vardır

Bazı durumlarda özellikle kullanıcı tarafından gelen verilerin tekil olarak saklanacağı listelere ihtiyacımız olabilir. İşte set tipini bu tip ihtiyaçlarda değerlendirebiliriz. Kullanımı oldukça basit olan Set sınıfına ait örnek bir kullanım şekli aşağıdaki kod parçasında yer almaktadır.

```text
require 'set'

sepetteki_urunler=Set.new

sepetteki_urunler<<"patates"
sepetteki_urunler<<"domates"
sepetteki_urunler<<"Domates"
sepetteki_urunler<<"biber"
sepetteki_urunler<<"patlican"
sepetteki_urunler<<"domates"
sepetteki_urunler<<"biber"

puts "Sepette neler var bir bakalim \n"
puts sepetteki_urunler.entries
```

![image.axd](images/image.axd)

set aslında hash algoritmasını kullanarak içerisine alınan elemanların tekil olmasını sağlayan bir tiptir. Tabii burada büyük küçük harf duyarlılığı da söz konusu. Örnek kod parçasında sepete eklenen bazı ürünler olduğunu görebilirsiniz. biber ve domates ikişer kez eklenmeye çalışılmış ama zaten veri yapısında olduklarından dahil edilmemişlerdir. domates ve Domates ise büyük küçük harf farkından dolayı ayrı kelimeler olarak değerlendirilmiştir. Atama işlemleri için << operatöründen yararlandığımıza lütfen dikkat edin. Ruby dilinin en sevdiğim özelliklerinden birisidir.

## Log Atmak mı? Hiç bu kadar kolay olmamıştı

Uygulamaların olmazsa olmaz ve vazgeçilmez ihtiyaçlarından birisidir Loglama. Kod akışının izlenmesi (trace), hataların ayıklanması (debug), sistemin durumunun anlık olarak öğrenilmesi (monitoring) ve daha pek çok yerde karşımıza çıkar bu Cross Cutting olarak adlandırdığımız mevzu. Ruby standart kütüphanesi loglama işini kolaylaştıran bir tip içermektedir. Ruby on Rails içerisinde de yer alan Logger sınıfının temel kullanımı ise aşağıdaki kod parçasında olduğu gibidir.

```text
require 'logger'

logger=Logger.new("uygulama.log")

logger.info "bu bir log testidir"
logger.warn "Saniyorum bir hata olustu"
logger.debug "Debug logu yazdiriyorum"
```

![image.axd](images/image.axd)

Gayet sade, gayet basit, gayet yalın. Logger sınıfına ait logger isimli nesne örneğini oluştururken dikkat edileceği üzere yapıcı metoda bir dosya adı verdik. Aslında burada parametre olarak STDOUT'da kullanabiliriz. Bunu kullandığımız takdirde log mesajları ekrana basılacaktır. Kodun ilerleyen kısımları da oldukça kolay. warn, info ve debug metodları tipik log seviyelerini belirtmektedir. Hemen hemen tüm loglama mekanizmalarında bu seviyelerin yer aldığını görebiliriz. Tabii logger sınıfının daha fazla detayı da mevcut. Söz gelimi yapıcı metoda süre tanımlamaları yaparak log dosyasının fazla şişmesinin önüne de geçebiliriz. Adamlar her şeyi düşünüyorlar.

```bash
# Sadece son bir aylik logu tut
Logger.new('app_1.log', 'monthly')  
# Bugunden itibaren gecmis 20 gunluk logu tut
Logger.new('app_1.log', 20, 'daily')  
# log boyutu 100mb uzerine cikarsa
Logger.new('app_1.log', 0, 100 * 1024 * 1024)
```

## Path bilgileri ile çalışmak

Özellikle dosya ve klasör işlemleri için kullanılabilen pathname sınıfı epey işe yarar fonksiyonellikler içermektedir. Çok basit anlamda aşağıdaki kod parçasında bir dosya ile ilişkili bilgileri nasıl alabiliriz, metin tabanlı içeriğini satır bazında nasıl okuyabiliriz ya da o an çalışmakta olduğumuz klasördeki öğeleri nasıl listeyebiliriz gibi temel işlemler icra edilmektedir.

```text
require 'pathname'

cwd = Pathname.getwd # current working directory
puts "Bulundugumuz klasordeki dosyalar\n"
cwd.each_child {|f| puts f}

pn = Pathname.new("uygulama.log")
puts "\n#{pn.basename} icin dosya boyutu #{pn.size} byte\n"
puts "Dosyanin tam yolu #{pn.expand_path}"
puts "Dosya uzantisi #{pn.extname}"
puts "Dosya icerigi\n"
pn.each_line {|line| puts line }
```

![image.axd](images/image.axd)

getwd isimli metod tahmin edileceği üzere uygulamanın çalışmakta olduğu klasörü döndürmektedir. eachchild metodu ile bu veya belirtilen diğer bir klasör içerisindeki öğeleri dolaşabiliriz. Belli bir dosya ile ilgili işlemler yapmak için yapıcı metoda dosya adını vermemiz yeterlidir. Sonrasında dosyanın boyutunu, sistemde yer aldığı klasörü, uzantısını ve hatta metin tabanlı içeriğini de ilgili fonksiyonlar ve alanlar ile yakalayabiliriz. basename, expandpath, extname, size özellikleri ile dosya adını, tam yol adını, dosya uzantısını ve byte cinsinden dosya boyutunu öğrenmekteyiz. eachline metodu da tahmin edileceği üzere dosya içeriğini satır bazında okumamıza yaramaktadır.

## O zaman biraz da matematik ve asal sayılar diyelim

Standart kütüphane asal sayılar ile ilgili Prime isimli bir tip içermektedir. Bu tip ile sadece bir ve kendisine bölünebilen sayılarla çalışmamız oldukça kolaydır. Örnek kod parçasına bir bakalım.

```text
require 'prime'

puts "5 asal mi? #{5.prime?}"
puts "3 asal mi? #{3.prime?}"
puts "8888 asal mi? #{8888.prime?}"

puts "Buyrun bunlar da 100 adet asal sayi"
print Prime.take(100)

total=0
Prime.each(11){|p|total+=p}
puts "\n11e kadar olan asal sayilarin toplami #{total} imis"
```

Sayısal değerlerin arkasına eklenen prime? çağrısı ile ilgili değerin asal sayı olup olmadığı bilgisi true veya false olarak elde edilebilir. Dilersek belirli sayıda asal sayıyı da çekebiliriz. take metodu aldığı parametre değeri kadar asal sayıyı sisteme geri verir. each metodu ile verilen asal sayıya kadar olan asallar elde edilebilir. Örnek kod parçasında 11 dahil 2,3,5,7,11 sayılarının toplamı hesaplanmıştır.

![image.axd](images/image.axd)

## Bir şeyleri ölçümleyelim mi?

Örneğin PI sayısını 100bin haneli olacak şekilde elde etmek istiyoruz. Acaba uygulamanın üzerinde çalıştığı sistem bunu ne kadar sürede yapabilir? Hemen öğrenmek ister misiniz? İşte gerekli kod parçası.

```text
require 'benchmark'
require 'bigdecimal/math'

puts Benchmark.measure{BigMath.PI(100_000)}
```

ve sonuçlar.

![image.axd](images/image.axd)

Tabii dört farklı değer gelmesi eminim sizi şaşırtmıştır. Bu değerler sırasıyla User CPU Time, System CPU Time, User CPU Time+System CPU Time ve son olarak Elapsed Real Time. Son derece basit öyle değil mi? Tahmin edileceği üzere {} blokları arasında ölçümlemesini yapmak istediğimiz kod parçasını yazmaktayız. Dolayısıyla burada iş yapan bir metod da kullanılabilir.

Aşağıdaki kod parçasını bu anlamda göz önüne alabiliriz.

```text
require 'find'
require 'benchmark'

def FindTotalSize()
	totalSize=0
	Find.find(Dir.pwd) { |p|
		totalSize += FileTest.size(p)
	}
	puts totalSize
	totalSize
end

puts Benchmark.measure{FindTotalSize()}
```

Bu kez FindTotalSize isimli metodun iş yapma hızı elde edilmiştir. Yeri gelmişken FindTotalSize metodu ne yapıyor bakalım. İçeride kullanılan Find modülüne ait olan find metodu ile o an çalışmakta olduğumuz klasördeki (Dir.pwd deki pwd=Present Working Directory anlamındadır) dosyaların toplam boyutu elde edilmeye çalışılır. Bunun için FileTest.size metodundan yararlanılır ve içerideki dosyaların boyutları üst üste eklenerek hesaplanır.

![image.axd](images/image.axd)

## CSV Formatlı dosyalar ile kolayca çalışalım

Veriyi text tabanlı olarak tutmak en eskiden beri bilinen yöntemlerdendir. Bu içeriği formatlı tutmak okunabilirlik ve verinin ayrıştırılarak kolayca anlaşılabilmesi açısından da önemlidir. Veriyi satırlar ve sütunlar halinde düşünüp çeşitli seperatörler ile ayrıştırmak gerekir. [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) (comma-seperated values) sık kullanılan dosya formatlarındandır. Ruby programlama dili CSV'ler ile kolay bir şekilde çalışabilmemizi sağlar. Yine en ilkel seviyede nasıl kullanıldığına bir bakalım dilerseniz. İşte örnek kod parçamız.

```text
require 'csv'

puts "Once information.csv icine bir seyler yazalim"
CSV.open("informations.csv","wb") do |file|

	file<<["id","title","category","listPrice"]
	file<<["1001","Locitek Fare","Bilgisayar Donanim","45 dolar"]
	file<<["1002","Deli Bilgisayar","Bilgisayar","1045 dolar + Tabii ki KDV"]
	file<<["1003","kespir tablet","Tablet","400 TL + KDV"]

end

puts "ve simdi de satir satir okuyalim"
CSV.foreach("informations.csv") do |line|
	puts line.inspect
end
```

![image.axd](images/image.axd)

Bir CSV dosyasını oluşturmak gayet kolaydır. Bunun için open metoduna uygun parametreleri vermemiz yeterlidir. Dosya açıldıktan sonra içerisine satır atmak çok daha kolaydır. << operatörünü kullanarak bu işlemi gerçekleştirebiliriz. CSV uzantılı bir içeriği okumanın farklı yöntemleri vardır. Bunlardan birisi de kod parçasında görülen foreach metodudur. foreach ilgili dosya içeriğini satır bazında okumaktadır.

Görüldüğü üzere standart kütüphane içerisinde bir çok işe yarar fonksiyonellik bulunmaktadır. Makalemizde değindiğimiz tiplerin daha pek çok özelliği bulunuyor. [Bunlar için mutlaka Ruby Dokümantasyonlarına bakmanızı öneririm](http://ruby-doc.org/stdlib-2.3.1/). Zaten Ruby dilini etkin kullanmak istiyorsak ilgili dokümanda belirtilen tipleri ele alıp gözümüze kestirdiklerimizi önceliklendirmek suretiyle çalışmamızda yarar olduğu kanısındayım. Nitekim çok fazla kütüphane bulunuyor. Böylece geldik bir Ruby Kod Parçacığımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

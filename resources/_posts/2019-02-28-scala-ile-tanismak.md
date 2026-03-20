---
layout: post
title: "Scala ile Tanışmak"
date: 2019-02-28 21:30:00 +0300
categories:
  - scala
tags:
  - scala
  - csharp
  - bash
  - dotnet
  - http
  - java
  - generics
  - visual-studio
  - github
---
Yazılım geliştirmek için kullanabileceğimiz bir çok programlama dili var. Hatta bir ürünün geliştirilmesi demek farklı platformların bir araya getirilmesi anlamına da geldiği için, bir firmanın kendi ekosisteminde çeşitli dilleri bir arada kullandığını görmek mümkün. [Scala](https://docs.scala-lang.org/tour/tour-of-scala.html)'da bu çark içerisinde kendisine yer edinmiş ve son zamanlarda dikkatimi çeken programlama dillerden birisi. Bunun en büyük sebebi daha önceden çalıştığım turuncu bankanın Hollanda kanadında servis odaklı süreçlerin orkestrasyonu için onu backend tarafında kullanıyor (veya deneyimliyor) olması. Hatta [şuradaki github adresinden](https://github.com/ing-bank/baker) açık kaynak olan Baker isimli ürünü inceleme şansımız da var. Scala ve Java kullanılarak yazılmış bir çatı.

![learn_scala_0.gif](/assets/images/2019/learn_scala_0.gif)

Bunu bir konuşma sırasında öğrenmiştim ancak inceleme fırsatını bir türlü bulamamıştım. O vaktiler süreç akışlarının yönetimi için.Net ve Tibco işbirliğinden yararlanılıyordu. Hollanda neden daha farklı bir yol izlemişti? Scala'yı tercih etme sebepleri neydi? Hatta Scala programcıları aradıklarına dair iş ilanları var. Bu sorular bir süre kafamı meşgul etmişti. Derken zaman geçti, hayatlar değişti, teknolojiler farklılaştı ve araştırılacak konu listesinde sıra ona geldi. Bir bakalım neymiş bu Scala dedikleri programlama dili.

Scala 2001 yılında Martin Odersky tarafından geliştirilmeye başlanmış ve resmi olarak 2004'te yayınlanmış. Java dilinin bir uzantısı gibi görünüyor ama kesinlikle değil. Scala ile yazılan kodlar Java Bytecode'a dönüştürülüp JVM üzerinde çalıştırılabiliyor ama aynı zamanda REPL modeline göre yorumlatılarak da yürütülebiliyorlar. Hal böyle olunca Java kütüphanelerinin kullanılabilir olduğunu söylemek şaşırtıcı olmaz sanıyorum ki. Dolayısıyla Scala içinden Java kullanımı ve tam tersi durum mümkün. Esasında onu Java ile birlikte sıklıkla anıyorlar. Java ile yapılan işlerin aynısını daha az kod satırı ile halledebileceğimizi söylüyorlar. Söz gelimi Java ile yazılmış Vehicle isimli aşağıdaki sınıfı düşünelim.

```csharp
public class Vehicle{
   private final String title;
   public Vehicle(String title){
      this.title=title;
   }
   public String getTitle(){
      return title;
   }
}
```

Bunu Scala tarafında şu haliyle yazmamız mümkün.

```text
case class Vehicle(title: String)
```

Scala ile ilgili övgüler bununla sınırlı değil tabii. Çok zarif bir şekilde nesne yönelimlilik (Object Oriented) ve fonksiyonel programlama paradigmalarını bir araya getirdiği belirtiliyor. Yani nesne yönelimli ilerlerken fonksiyonel olmanın kolaylıklarını da ele abiliriz gibime geliyor. Bu sebeptendir ki Scala'daki her şey birer nesnedir. Buna sayılar gibi tipler haricinde fonksiyonlar da dahildir. Genel amaçlı bu dilin pek çok fanatiği var. Söz gelimi Twitter'ın [şu adreste yayınlanmış repo'larında](https://github.com/twitter?language=scala) Scala sıklıkla geçiyor.

> Eğer yeni mezunsanız veya halen öğrenciyseniz ve yeni bir programlama dili öğrenmek istiyorsanız size "Scala'yı mutlaka öğrenin" diyemem. Size C#'ı yutun, Java'da efsane olun da diyemem. Ancak size sıklıkla kullandığınız Facebook, Twitter, Instagram, Linkedin, Netflix, Spotify gibi devlerin Github repolarına bir bakın derim. Neyi çözmek için hangi ürünlerinde neleri kullanmışlar, sizlere birçok fikir verecektir. Asla tek bir dilin veya platformun fanatiği olmamak lazım. Bunların hepsi birer araç. Aşağıdaki görselde yazıyı hazırladığım tarih itibariyle baker projesinin içerisindeki dil kullanım oranlarını görüyorsunuz. Scala ve Java bir arada ele alınarak geliştirilmiş bir çatı.
> ![learn_scala_1.gif](/assets/images/2019/learn_scala_1.gif)

Benim şu anki amacım dili temel özellikleri ile tanımaya çalışmak, bir kaç satır kod yazıp el alışkanlığı kazanmak. Java ile yazılım geliştirenler için öğrenirken yazım stiline alışmakta zorluk yaşandığına dair söylentiler var. Java ile yıllardır uğraşmamış bir.Net geliştiricisi olarak beni de epey zorlayacak diye düşünüyorum. Haydi gelin West-World'de (Ubuntu 16.04'ün 64 Bit sürümü olduğunu biliyorsunuzdur artık), Visual Studio Code kullanarak dili tanımaya çalışalım. İşe terminalden bağzı kurulumları yaparak başlamak gerekiyor elbette. Öncelikle sistemde JDK 8 (v 1.8 olarak da biliniyor) yüklü olmalı. West-World'de yüklüydü ki bunu versiyonu kontrol ederek teyid ettim. Sonrasında Scala'nın kurulumunu yaptım. İşte kullanabileceğimiz terminal komutları.

```bash
## Java SDK Kurulumu
sudo apt-get update
sudo apt-get install default-jdk

## Scala Runtime Kurulumu
sudo apt-get remove scala-library scala
sudo wget http://scala-lang.org/files/archive/scala-2.12.6.deb
sudo dpkg -i scala-2.12.6.deb
sudo apt-get update
sudo apt-get install scala
```

Muhtemelen sisteminizde JDK yüklüdür ama her ihtimale karşı genel bir güncelleme ve sonrasında JDK kurulumu ile işe başlanabilir. Ardından var olan bir scala sürümü varsa bunu kaldırmanızı öneririm. Öğrenmeye başlarken son stabil sürüm ile ilerlemekte yarar var. deb paketini indirdikten sonra bunu açıp kuruyoruz. Bu işlemler başarılı bir şekilde gerçekleştiyse terminalden scala yazarak yeni bir ufka doğru yelken açmaya başlayabiliriz. Örneğin aşağıdaki ifadeleri deneyebiliriz.

```text
scala
nickname="persival"
var nickname="persival"
nickname
nickname="perZival"
nickname
val default_point=50,45
val default_point=50.45
default_point
default_point=60.50
```

Scala arabirimine ulaştıktan sonra ilk önce bir değişken tanımlayayım istedim. Tür belirtmeden bodoslama yazdım. Tabii ki hata aldım. Bunun üzerine en başta yapmam gereken şeyi yaptım. Dokümanı okumaya başladım. Sonrasında var ile val şeklinde bir kullanıma rastladım. var ile değeri değiştirilebilir (mutable) bir değişken tanımlanabildiğini anladım. val komutuyla da değeri değiştirilemeyen (immutable) değişkenlerin tanımlanabileceğini öğrendim ki default_point'in değerini bu sebeple değiştiremedim. Aralarda ufak yazım hatalarım da oldu. double bir değişken için virgül kullanımı hata verdi örneğin. Genel olarak ilk sonuçlar aşağıdaki ekran görüntüsündeki gibi oldu.

![learn_scala_2.gif](/assets/images/2019/learn_scala_2.gif)

İlerlemeye devam ettim ve değişken tanımlarken kullanılabilen def ifadesiyle karşılaştım. Önce mutable bir değişkenle, sonradan immutable bir tanesiyle denedim.

```text
var city="istanbul"
def label_city=city
label_city
city="Istanbul"
label_city
label_city="istanbul"
```

Burada dikkat çekici bir nokta var. city isimli değişken için def ile bir başka değişken tanımlanıyor. Ancak label_city değişkeninin içeriği o çağrılana kadar alınmıyor. Bir başka deyişle değer ataması çağrım yapıldığı zaman gerçekleştiriliyor (Oysa ki yukarıdaki var ve val kullanımlarında atama yapılır yapılmaz değişkenler atanan değerlerine sahip olmuşlardı) Bunun henüz ne işe yarayacağını bilemiyorum ancak dili tasarlayanların mutlaka bir amacı vardır. Ayrıca city değişkeninin değerinde yapılacak değişiklikler label_city'yi de etkiliyor. Aynı referansa bakan değişkenler olduklarını ifade edebiliriz. Nitekim label_city değişkenine değer ataması yapamıyoruz.

![learn_scala_3.gif](/assets/images/2019/learn_scala_3.gif)

Bu birkaç kod parçasında String ve Double tipleri ile tanışmış oldum. Elbette başka tipler de mevcut. Boolean, Byte, Short, Int, Long, Float, Char diğer veri tiplerinden. Aslında tip ağacının tepesinde Any yer alıyor. Any'den türeyen AnyVal ve AnyRef isimli iki ana alt tip daha var. Tüm tiplerin bu ikisinden türediğini söyleyebiliriz. AnyVal değer türleri için AnyRef ise referans türleri için ata tip olarak ele alınmakta. Yeri gelmişken Scala'nın case-sensitive bir dil olduğunu belirtelim. Şunları bir deneyin mesela;

```text
var isExist=true
isExist=False
VAR name="what"
val point:int=40
val point:Int=40
```

![learn_scala_4.gif](/assets/images/2019/learn_scala_4.gif)

> Komut satırından Scala dilinin özelliklerini öğrenmeye çalışırken ekranı temizlemek için CTRL+L tuş kombinasyonundan yararlanabileceğimizi öğrendim. Ayrıca:help ile kullanabileceğimiz önemli terminal komutlarını da görebiliriz. Söz gelimi:reset ile o ana kadar ki tüm REPL işlemlerini sıfırlamış olur ve başlangıç konumuna döneriz.

Diğer dillerde olduğu gibi Scala içerisinde de if-else if-else kullanımları mevcut. Nam-ı diğer koşullu ifadeler. Ancak bunun yerine aşağıdaki kod parçasına baksak daha güzel olur.

```text
def isEven(number:Int) = if (number%2==0) "Yes" else "No"
```

Burada isEven isimli bir değişken tanımlanmış durumda ki aslen kendisi bir metod ve eşitliğin sağ tarafındaki ifadenin sonucunu taşıyor. Bu örneği daha çok Scala'da ternary (?:) operatörünün olmayışına karşılık gösteriyorlar. Yukarıdaki fonksiyona ait örnek bir kullanımı aşağıdaki ekran görüntüsünde bulabilirsiniz.

![learn_scala_5.gif](/assets/images/2019/learn_scala_5.gif)

Tabii if-else if-else kullanımını görünce insan switch case gibi bir bloğu da arıyor. Tam olarak aynı isimde olmasa da Pattern Matching özelliği kullanılarak bu mümkün kılınabiliyor. Bir anlamda bir değeri bir desenle kıyaslayıp kod akışını yönlendiriyoruz. Örneğin şu kod parçasında olduğu gibi.

```text
def gradeValue(point:Int):String = point match {
     case 1 => "A"
     case 2 => "B"
     case 3 => "C"
     case _ => "you should work more"
     }
gradeValue(1)
gradeValue(3)
gradeValue(5)
```

gradeValue fonksiyonu için Pattern Matching kullanılıyor. point değişkeninin değerine göre case ifadelerinden birisi çalışıyor. case _ kısmı, Alternatives olarak isimlendirilen bölüm. point değişkeninin 1,2 ve 3 dışındaki tüm değerleri için çalışıyor.

![learn_scala_6.gif](/assets/images/2019/learn_scala_6.gif)

Ancak dahası var. Sınıflar ile birlikte kullanıldığı bir senaryo. Bu senaryoyu denemek için terminali terketmek gerektiğini düşünüyorum. Visual Studio Code ile ilerlemek daha doğru olacaktır. O zaman burada kısa bir es verelim derim. Visual Studio Code'da scala kodu nasıl yazılabilir öğrenelim isterim. Öncelikle bugün ve sonrası için West-World üzerinde bir klasör açtım. Sonrasında scala uzantılı HelloWorld isimli aşağıdaki kod içeriğine sahip dosyayı oluşturdum. Code dosyayı tanıyarak bana hemen bir uzantı (extension) önerdi. Onu yükleyerek devam ettim ve aşağıdaki içeriği oluşturdum.

```text
object HelloWorld {
  def main(args: Array[String]) {
    println("Hello from Scala")
  }
}
```

Sonrasında aşağıdaki terminal komutları ile kod dosyasını derleyip çalıştırdım!

```bash
scalac HelloWorld.scala
scala -classpath . HelloWorld
```

![learn_scala_7.gif](/assets/images/2019/learn_scala_7.gif)

Aslında burada Scala kodunun derlenerek çalıştırılması söz konusu. Zaten kod içeriğine bakılacak olursa C#,C, C++ ve Java gibi dünyalardan pekala aşina olduğumuz main metodumuz var. Ekrandan parametre alabilen ve bunları bir String dizi üzerinden içeriye alan bu metod program çalıştırıldığındaki giriş noktası görevini üstleniyor. Scala'nın aynı zamanda yorumlamalı olarak da çalışabileceğinden bahsetmiştik. Yani aşağıdaki gibi bir kullanım da söz konusu olabilir.

```text
println("Merhaba benim adım Burak")
println("Bugün",java.time.LocalDate.now)
val pi=3.14
var r=2
var result=pi*r*r
println(result)
```

Tutorial_1.scala ismiyle kaydettiğim dosyayı çalıştırmak için,

```bash
scala Tutorial_1.scala
```

terminal komutunu vermek yeterliydi. Kod içeriği yorumlanarak çalıştırılacaktı.

![learn_scala_8.gif](/assets/images/2019/learn_scala_8.gif)

Satır satır yorumlanarak kodun çalıştırıldığını görüyoruz. İstersek derleyerek, istersek yorumlatarak çalışabileceğimizi görmüş olduk. Hımmm. Etkileyici;)

Artık scala terminalini terk etmenin zamanı gelmişti. Dili öğrenmek için Visual Studio Code arabiriminden devam edebilirdim. Tekrar Pattern Matching konusuna döndüm ve bu kez aşağıdaki kod içeriğini yazdım.

```text
abstract class Messenger

case class Human(to: String, title: String, message: String) extends Messenger
case class Computer(to: String, message: String) extends Messenger
case class Broadcast(message: String) extends Messenger

def sendAlert(messenger: Messenger): String = {
  messenger match {
    case Human(to, title, message) =>
      s"This message for you dear $to. Title : $title. Message : $message"
    case Computer(to, message) =>
      s"This message for you dear $to. Message : $message"
    case Broadcast(message) =>
      s"To all units '$message'"
  }
}
val jordi = Human("Peter", "Dude. What's up?","I am going to go there next weekend body :)")
val microServiceCenter = Computer("Sam", "The CPU service is not responding at this moment")
var westWorldCon=Broadcast("Emergency drop. Delete everything")

println(sendAlert(jordi))
println(sendAlert(microServiceCenter))
println(sendAlert(westWorldCon))
```

Messenger soyut bir sınıf ve diğer üç case class bu tipten türüyor. Tipik bir kalıtım söz konusu diyebiliriz sanırım. Human, Computer ve Broadcast sınıfları pek normal sınıf formatında değiller aslında değil mi? Dikkat ederseniz case class bildirimi ile tanımlanmış durumdalar ve hatta doğrudan parametre alıyorlar. Bakmayın ben direkt sınıf olduklarını ifade ettim. sendAlert isimli metod parametre olarak Messenger tipinden bir değişken alıyor. Buna göre ilgili metod Human, Computer ve Broadcast tipleri ile çalışabilir ki çok biçimli (Polymorphism) bir yapıda olduğunu ifade etsek yanlış olmaz. Eşitliğin sağ tarafına göre metod String değer dönürecek. Metodun kullanımı sırasında case class'lara ait birer değişkenin parametre olarak verildiğini görebilirsiniz. jordi, microServiceCenter ve westWorldCon bu kod parçasındaki kahramanlarımız. sendAlert metodu kendisine gelen case class kimse eşleşmeye göre ona ait kod bloğu çalıştırılmakta. s ile çift tırnaklı ifadeyi formatlıyoruz ve $ ile başlayan değişken adları aslında parametreleri işaret ediyor.

![learn_scala_9.gif](/assets/images/2019/learn_scala_9.gif)

İşler büyümeye başladı değil mi? if-else if-else derken pattern matching gibi değişik bir konuya denk geldik. Üstelik Pattern Matching bu kadarla da sınırlı değil. Guard, sealed class denilen kavramlar söz konusu. Hele ki case class olgusu. Normal sınıf olarak düşünebileceğimiz case class'lar aslında immutable veri tiplerinin modellenmesinde kullanılmak üzere düşünülmüşler. Onu da merak edip devam etmek istiyorum ancak oraya gelmek için başka şeyleri de öğrenmem gerekiyor. Mixin, trait, case class vs Sakin olmanın tam sırası. Böylesine ciddi bir dil çat diye öğrenilemez. Basit adımlarla dili tanımaya devam etmem lazım. Nitekim gözden kaçırdığım detaylar var. Söz gelimi fonksiyon ve metod aslında iki ayrı kavram. Aşağıdaki kod parçasını göz önüne alarak ilerleyelim.

```text
var sayHello= (name:String) => println(s"Merhaba $name")
sayHello("Burak")

val sum = (x:Int,y:Int) => x+y
println(sum(4,5))

def diff(a:Int,b:Int):Int = a-b
println(diff(6,7))

def writeAndCombine(name:String)(message:String):String = {
    var output=s"$name! Sana bir mesaj var. '$message'"
    return output
}

println(writeAndCombine("Persival")("Bugün nasılsın?"))

def getState: String = "All is well"
var state=getState
println(state)
```

Örnekte basit fonksiyon ve metod kullanımları var. Metodları def anahtar kelimesinden ayırt edebilirsiniz. sayHello ve sum birer fonksiyon olarak karşımıza çıkıyorlar. Metodlar, fonksiyonlardan farklı olarak geri dönüş değerleri varsa tipinin ne olduğunu belirtmekte. Fonksiyonlar için bu durum söz konusu değil. Ayrıca bir metodun çoklu (birden fazladan farklı bir durum) parametre listesine sahip olması da söz konusu. writeAndCombine metoduna dikkat ederseniz bu durumu göreceksiniz. Fonksiyonları var ve val kelimeleri ile tanımlayabiliyoruz. Metodlar ise mutlak suretle def ile tanımlanmalılar. Her ne kadar writeAndCombine metodunda return kelimesini kullanmış olsak da bu mecburi değil. Fonksiyonlar aslında => operatörünün sağ tarafındaki ifadenin bir değişkene atanmış hali gibi düşünülebilirler. Fonksiyonları isimsiz olarak tanımlamak da mümkün (Anonymous Function) Arada kullanım yerleri göz önüne alındığında başka farklılıklar olduğuna da eminim. Nitekim hangi durumlarda fonksiyon hangi durumlarda metod tercih edilir bilmek lazım. Bendeki çalışma zamanı çıktıları aşağıdaki gibi oldu.

![learn_scala_10.gif](/assets/images/2019/learn_scala_10.gif)

Aslında metodları bir sınıf içerisinde deneyimlesek güzel olabilir değil mi? O zaman bir sınıf tanımlayalım.

```text
class ContextManager(conStr:String)
{
    def GetActorCount(query:String):Int={
        println("Sistemdeki aktor sayısı bulunacak")
        println(s"Sorgumu '$query'")
        148
    }

    def Ping():Unit = println("Pong")

    val ConnectionString = () => conStr
}

var morinyo=new ContextManager("server=manchester;db=players;u_id=admin,pwd=1234")
morinyo.Ping()
var actorCount=morinyo.GetActorCount("select * from players where state='actor'")
println(actorCount)
println(morinyo.ConnectionString())
```

ContextManager isimli bir sınıfımız var. Sınıf tanımı sırasında parametre verebildiğimizi fark etmişsinizdir. conStr aslında yapıcı metod parametresi gibi düşünülebilir. İçerideki ConnectionString fonksiyonu onu doğrudan kullanmaktadır da;) Ortada görünen bir yapıcı metod ya da parametrelerini aktardığımız özellikler yok ancak kullanabiliyoruz. Bir şeylerin basitleştirildiği kesin. GetActorCount bir metod ve parametre olarak gelen sorguyu çalıştırıp Integer bir değer dönüyor. Hayali olarak elbette. Ping isimli metodumuzun dönüş tipi ise dikkatinizi çekmiş olmalı. Unit, void gibi anlamlandırılan bir dönüş tipiymiş. Şimdilik bunu öğrendim. ContextManager sınıfına ait nesne örneği oluşturmak için new operatöründen yararlanılıyor. Sonrasında ise bildiğimiz metod ve fonksiyon çağrımları söz konusu. İşte çalışma zamanı sonuçları.

![learn_scala_11.gif](/assets/images/2019/learn_scala_11.gif)

Yazının bu kısmında bıraksam mı yoksa hazır sınıflara değinmişken kısaca Case Class türüne bir baksam mı düşünmeye başladım. Filtre kahvem bitmişti zaten. Yenilerken soluklanır ve sonra tam gaz dokümanlar üzerinden araştırmaya devam ederim diye düşündüm. Öyle de yaptım:) Case Class aslında adı üzerinde "Kasa sınıf" olarak düşünülmeli. Bu özel tip immutable sınıflar tanımlamamıza olanak sağlıyor. Bu sebeple de değer bazlı (Compare by Value) karşılaştırmalar söz konusu. Aslında sınıfları referans, kasa sınıflarını da değer türü gibi düşünebiliriz (class vs struct gibi. Bu dillerde ne çok birbirlerine benziyorlar değil mi?) Aşağıdaki kod parçasını kullanarak bu durumu anlamaya çalışabiliriz.

```text
case class Dimension(x:Double,y:Double,z:Double)
class Dim(x:Double,y:Double,z:Double)

val location1=Dimension(3.4,5.2,9.1)
val location2=Dimension(3.4,5.2,9.1)

println(location1 == location2)

val loc1=new Dim(3.4,5.2,9.1)
val loc2=new Dim(3.4,5.2,9.1)

println(loc1 == loc2)
```

![learn_scala_12.gif](/assets/images/2019/learn_scala_12.gif)

Dimension bir case class. Dim ise normal bir sınıf olarak tanımlandı. location1 ve location2 birer Case Class örneği ve aynı x,y,z değerlerine sahipler. loc1 ve loc2 ise birer Dim sınıf örneği ve x,y,z bazında yine aynı değerlere sahipler. Ancak karşılaştırılma durumları farklı. Case Class'lar değer bazlı olarak karşılaştırıldığından sonuç true. Sınıflar içinse tam aksi durum söz konusu ki bu normal.

Scala'yı anlamak için bulduğum dokümanları kurcalarken dilin temel özellikleri içerisindeki Object ve Trait kavramları da ilgimi çekti. Bir sınıfın sadece tek bir nesne örneğine sahip olmasını istiyorsak Object tipinden yararlanabiliriz. Çalışma zamanı ona ihtiyaç duyulduğu yerde oluşturacaktır (Lazy creation) Bir nevi Singleton nesne tanımlamak için kullanılan basit bir tip olarak düşünebiliriz. Aşağıdaki kod parçasında object tipinin kullanımına ilişkin basit bir deneme var.

```text
object Listener {
  def Start:Boolean = { 
    println("listening...") 
    true
    }
  def Stop:Boolean = { 
    println("stoped!") 
    true
    }
}

Listener.Start
Listener.Stop
```

![learn_scala_13.gif](/assets/images/2019/learn_scala_13.gif)

Pek tabii Singleton nesnelere ihtiyaç duyulan senaryolara bakmak lazım buradaki durumu anlamak için. Şu anda sadece yazım stili ve kavramsal olarak farkındalık sahibi oldum diyebilirim. Gelin birde şu Trait mevzusuna bakalım. Trait'ler belirlenmiş alan ve metodları taşıyorlar. Örneklenemeyen bir tür ancak başka sınıf veya nesneleri genişletmekte kullanılabiliyorlar. Java 8 tarafındaki Interface tipine benzetiliyorlar (İnceleyince çok uzun zamandır C# tarafında var olan interface'lerden ne farkı var anlayamadım tabii ama Scala çalıştıkça fark edeceğim diye düşünüyorum) Generic tipleri de Trait'ler ile değerlendirmek mümkün. Aşağıdaki kod parçası onun kullanımı ile ilgili bana temel bir fikir verdi aslında.

```text
trait Capability {
    def Walk(stepCont:Int)
    def Stop:Boolean
    def Turn(location:String)
}

class Truck(title:String) extends Capability{
    override def Walk(stepCount:Int) = println(s"$stepCount walk")
    override def Stop:Boolean=true
    override def Turn(location:String) = println(s"go to $location")
}

trait SpecialCapability extends Capability{
    def Fire(target:String):Boolean
}

val v40=new Truck("Volvi v40")
v40.Walk(10)
v40.Turn("Montreal")
```

![eWnzaQAAAABJRU5ErkJggg==](data:image/png;base64,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)

Capability içerisinde Walk, Stop, Turn isimli metod tanımlamaları var. Capability'den genişletilen Truck sınıfı bu metodları uygulamak zorunda. Diğer yandan SpecialCapability isimli Trait ek bir metod tanımı daha getiriyor. Yani Capability Trait'ini genişletmiş olduk.

Scala dili tabii ki birkaç sayfada anlatılamaz. Ben merak ettiğim için bu dili incelemeye çalışıyorum. Daha fazla hakim olmak için gerçek hayat senaryolarında kullanmak ve en azından bir kurumsal çaplı projede değerlendirmek lazım. Yine de ilk yazı için onun hakkında bir takım fikirler elde edebildiğimi düşünüyorum. Başlarda da belirtiğim üzere büyük oyuncularının dikkatini çeken ve ürünlerinde kullandıkları bir programlama dili olarak karşımıza çıkıyor. Scala ile ilgili yeni veya ilginç şeyler öğrendikçe buradan paylaşmaya devam etmeyi düşünüyorum. Scala'yı çalışmak için ilk kaynak olarak [resmi sitesinden](https://docs.scala-lang.org/tour/tour-of-scala.html) yararlanıyorum ancak Doğuş Teknoloji sağolsun Pluralsight eğitimleri de epey yardımcı oluyor. Ayrıca [Martin Odersky'nin Programming in Scala](https://www.amazon.com/Programming-Scala-Updated-2-12/dp/0981531687/ref=sr_1_1?s=books&ie=UTF8&qid=1535901044&sr=1-1&keywords=scala+cookbook) kitabını da şiddetle tavsiye ederim. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

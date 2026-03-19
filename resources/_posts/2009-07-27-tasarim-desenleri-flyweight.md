---
layout: post
title: "Tasarım Desenleri - FlyWeight"
date: 2009-07-27 09:30:00 +0300
description: "Yandaki resimde yer alan minik boksör aslında hafif siklette mücadele etmektedir ve biraz sonra işleyeceğimiz FlyWeight tasarım kalıbı ile uzaktan yakında hiç bir alakası bulunmamaktadır Sealed Ancak işleyeceğimiz tasarım kalbına bu ismin verilmesininde bir sebebi vardır. Bakalım neymiş?"
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - generics
---
# Tasarım Desenleri - FlyWeight
![blg51_2.jpg](/assets/images/2009/blg51_2.jpg)

Merhaba Arkadaşlar,

Yandaki resimde yer alan minik boksör aslında hafif siklette mücadele etmektedir ve biraz sonra işleyeceğimiz FlyWeight tasarım kalıbı ile uzaktan yakında hiç bir alakası bulunmamaktadır

![Sealed](/assets/images/2009/smiley-sealed.gif)

Ancak işleyeceğimiz tasarım kalbına bu ismin verilmesininde bir sebebi vardır. Bakalım neymiş?

Yapısal (Structural) tasarım kalıplarından olan FlyWeight, bellek tüketimini optimize etmek amacıyla kullanılan bir desendir. Aslında detayına inildiğinde son derece zekice tasarlanmış ve pek çok noktada karşımıza çıkabilecek havuz mantığını içeren bir kalıp olduğu anlaşılabilir. Burada önemli olan nokta, bellek tüketiminin çok fazla sayıda nesnenin bir arada ele alınması sırasında ortaya çıkmasıdır. Buna göre söz konusu nesnelerin ortak olan, paylaşılabilen içerikleri ve bunların dışında kendilerine has durumları olduğu takdirde, nesne üretimlerini sürekli tekrar ettirmektense basit bir havuz içerisinden tedarik ettirmek, uygulamanın harcadığı bellek alanlarının optimize edilmesi için yeterli olacaktır. Bu açıdan bakıldığında desenin, paylaşımlı nesneleri efektif olarak kullanabilmek üzerine odaklandığını söyleyebiliriz.

Aslında, FlyWeight tasarım kalıbını hangi kaynaktan araştırırsak araştıralım ilk etapta dikkat edilmeyen çok önemli bir özellik içermektedir. Her bir FlyWeight nesnesi temel olarak iki önemli veri kümesinden oluşur. Kaynaklarda çoğunlukla intrinsic olarak geçen durum-bağımsız (State-Independent) kısım parçalardan birisir. Bu kısımda, çalışma zamanındaki tüm FlyWeight nesneleri tarafından saklanan paylaşılmış alanlar yer almaktadır. Diğer parça ise durum-bağımlı (State-Dependent) olarak bilinen ve kaynaklarda çoğunlukla extrinsic olarak belirtilen kısımdır. Bu kümedeki veriler ise istemci tarafından saklanır, hesap edilir ve FlyWeight nesne örneğine, yine FlyWeight'in bir operasyonu yardımıyla aktarılırlar. Tabiki desenin kullanım amacını daha net bir şekilde kavrayabilmek için bazı örnek senaryolar üzerinden gitmeye çalışabiliriz.

Bu desen ile ilişkili en belirgin örnek kelime işlemcilerinde ortaya çıkmaktadır. Nesneye dayalı olarak geliştirilen bir kelime işlemcisinde her bir karakterin nesne olarak oluşturulduğunu düşünelim. Her bir karakterin döküman içerisinde çok fazla sayıda kullanılabileceği ortadadır. Dolayısıyla aynı ortak özelliklere sahip olan bir çok karakter nesne örneğinin yönetimi söz konusudur ve buda doğal olarak bellek üzerinde daha fazla yer harcanmasına neden olacaktır. Aynı şekilde, bu nesnelerin tekrardan oluşturulmalarının maliyetide doğrudan performansa yansıyacaktır. Oysaki bu karakter nesnelerinin pek çoğu için ortak olan bir takım veriler söz konusudur. Örneğin, karakterlerin boyutları, font tipleri, büyüklükleri vb...Bunların dışında kelime işlemci açısından önem arz eden ve karakter nesneleri tarafından ortak olarak düşünülemeyecek bir takım verilerde vardır. Söz gelimi karakterlerin lokasyonu aslında istemci tarafından (yani kelime işlemci uygulama) belirlenebilir. Dolayısıyla karakterleri temsil eden tipler aslında ortak özellikleri bir yerde toplanıp, örnekleride havuzlanarak hafifleştirilebilir. Hafifleştirmek aslında bu desene neden FlyWeight adının verildiğinide ortaya koymaktadır.

Biz örnek uygulamamızda benzer bir senaryoyu ele alıyor olacağız. Senaryomuzda bir oyun sahnesinde yer alan çok sayıda asker olduğunu düşünüyoruz. Örnek olarak er ve çavuşları göz önüne alacağız. Bunların çok sayıda olduğunu ve sürekli tekrar eden sayısız örneklerinin uygulama alanında değerlendirildiğini göz önüne alırsak, oyun sahnesine getirdikleri bellek yükünü hafifletmek amacıyla, söz konusu asker nesnelerini birer FlyWeight tip haline getirmeyi deneyeceğiz. Sonrasında ise bu askerlerin oyun sahnesindeki yüklerini dengeleyecek, bir başka deyişle havuzu oluşturup, istemciye sunacak bir fabrika tipi (FlyWeight Factory) tasarlayacağız. Elbette öncesinde FlyWeight deseninin genel UML şemasına bakmamızda yarar var.

![blg51_uml.gif](/assets/images/2009/blg51_uml.gif)

UML şemamızda gördüğümüz üzere FlyWeightFactory nesnesi ile FlyWeight nesnesi arasında bir Aggregation söz konusudur. Bu son derece doğaldır nitekim fabrikamız, kendi içerisinde yer alan bir depolama alanı ile FlyWeight nesne örneklerini havuzlamakta ve istemcinin ihtiyacı olan FlyWeight nesne örneklerini bu havuzdan tedarik etmektedir. Bu noktada istemci (Client) ile, FlyWeight Factory ve Concrete FlyWeight nesneleri arasında tek yönlü bir Association söz konusudur. Yani, Client diğerlerinin nesne örnekleri ve içeriklerini kullanmaktadır. Concrete FlyWeight tipi, türeyenler için Intrinsic state verileri ile Extrinsic state verilerinin ele alındığı ortak operasyonu tanımlamaktadır. Interface veya abstract sınıf tipinden tasarlanabilir.

Artık örneğimizi geliştirmeye başlayabiliriz. İşte sınıf diagramımız;

![blg51_1.gif](/assets/images/2009/blg51_1.gif)

Ve kodlarımız;

```csharp
using System.Collections.Generic;
using System;

namespace FlyWeight
{
    enum SoldierType
    {
        Private,
        Sergeant
    }

    // FlyWeight Class
    abstract class Soldier
    {
        #region Intrinsic Fields

        // Bütün FlyWeight nesne örnekleri tarafından ortak olan ve paylaşılan veriler
        protected string UnitName;
        protected string Guns;
        protected string Health;

        #endregion

        #region Extrinsic Fields

        // İstemci tarafından değerlendirilip hesaplanan ve MoveTo operasyonua gönderilerek FlyWeight nesne örnekleri tarafından değerlendirilen veriler
        protected int X;
        protected int Y;

        #endregion

        public abstract void MoveTo(int x, int y);
    }

    // Concrete FlyWeight
    class Private
        : Soldier
    {
        public Private()
        {
            // Intrinsict değerler set edilir
            UnitName = "SWAT";
            Guns = "Machine Gun";
            Health = "Good";
        }
        public override void MoveTo(int x, int y)
        {
            // Extrinsic değerler set edilir ve bir işlem gerçekleştirilir
            X = x;
            Y = y;
            Console.WriteLine("Er ({0}:{1}) noktasına hareket etti", X, Y);
        }
    }

    // Concrete FlyWeight
    class Sergeant
        : Soldier
    {
        public Sergeant()
        {
            UnitName = "SWAT";
            Guns = "Sword";
            Health = "Good";
        }
        public override void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
            Console.WriteLine("Çavuş ({0}:{1}) noktasına hareket etti",X,Y);
        }
    }

    // FlyWeight Factory
    class SoldierFactory
    {
        // Depolama alanı(Havuz).
        // Uygulama ortamında tekrar edecek olan FlyWeight nesne örnekleri depolama alanında basit birer Key ile ifade edilir
        private Dictionary<SoldierType, Soldier> _soldiers;

        public SoldierFactory()
        {
            _soldiers = new Dictionary<SoldierType, Soldier>();
        }

        public Soldier GetSoldier(SoldierType sType)
        {
            Soldier soldier = null;

            // Eğer depolama alanında, parametre olarak gelen Key ile eşleşen bir FlyWeight nesnesi var ise onu çek
            if (_soldiers.ContainsKey(sType))
                soldier = _soldiers[sType];
            else
            {
                // Yoksa Key tipine bakarak uygun FlyWeight nesne örneğini oluştur ve depolama alanına(havuz) ekle
                if (sType == SoldierType.Private)
                    soldier = new Private();
                else if (sType == SoldierType.Sergeant)
                    soldier = new Sergeant();
                _soldiers.Add(sType, soldier);
            }

            // Elde edilen FlyWeight nesnesini geri döndür
            return soldier;
        }
    }

    class Program
    {
        public static void Main()
        {
            // İstemci için örnek bir FlyWeight nesne örneği dizisi oluşturulur
            SoldierType[] soldiers = { SoldierType.Private, SoldierType.Private, SoldierType.Sergeant, SoldierType.Private, SoldierType.Sergeant };

            // FlyWeight Factory nesnesi örneklernir
            SoldierFactory factory = new SoldierFactory();

            // Extrinsic değerler set edilir
            int localtionX = 10;
            int locationY = 10;

            foreach (SoldierType soldier in soldiers)
            {                
                localtionX += 10;
                locationY += 5;
                // O anki Soldier tipi için MoveTo operasyonu çağırılmadan önce fabrika nesnesinden tedarik edilir
                Soldier sld = factory.GetSoldier(soldier);
                // FlyWeight nesnesi üzerinden talep edilen operasyon çağrısı gerçekleştirilir
                sld.MoveTo(localtionX, locationY);
            }
        }
    }
}
```

Dilerseniz örneğimizi kısaca incelemeye çalışalım.

FlyWeight haline getirilen Private ve Sergeant isimli sınıflarımız abstract Soldier sınıfından türemektedir. Soldier sınıfı bu desene göre FlyWeight tipi görevini üstlenmekte olup kendisinden türeyen Private ve Sergeant tipleri asıl FlyWeight tiplerinin (Concurrent FlyWeight) modelleridir. Soldier tipi içerisinde bir askerin ortak alanları tutulmaktadır. Bunlardan UnitName, Guns ve Health özellikleri aslında içsel durumu (Intrinsic State) ifade etmektedir.

Bir başka deyişle tüm benzer askerler için ortak ve paylaşılan bilgiler olarak düşünülmektedir. Tabiki senaryo gereği. Diğer yandan bir askerin, oyun sahası üzerindeki lokasyonu X ve Y isimli alanlarda tutulmaktadır. Bu alanların değerleri istemci açısından önemlidir. Nitekim oyun sahasında aynı askerin birden fazla örneği olabilmesine rağmen lokasyonları çeşitlilik gösterebilir. Bu nedenle X ve Y alanları aslında bir askerin harici durumu (Extrinsic State) ile alakalıdır. Peki bu durum nasıl değerlendirilir?

Desenin uygulanış biçimi gereği Extrinsic State içeriği, FlyWeight nesne örnekleri içerisine bir operasyon yardımıyla aktarılır ve değerlendirilir. İstemci tarafından gerçekleştirilecek bu operasyon çağrısı, örnek senaryomuzda MoveTo isimli metod ile ifade edilmektedir. Desenin belkide en önemli aktörlerinden biriside SoldierFactory (FlyWeight Factory) isimli sınıftır. Bu sınıf içerisinde dikkat edileceği üzere askerlerin birer anahtar ile saklanabilmeleri ve bu sayede birden fazla sayıda olan aynı FlyWeight nesnesinin tek bir sembol ile ifade edilebilmeleri mümkündür. Bunun için basit bir Dictionary koleksiyonundan yararlanılmaktadır.

Bu koleksyion tam olarak, FlyWeight nesne havuzunun kendisidir. Peki istemci tarafının talep edeceği Soldier (FlyWeight) nesne örnekleri nasıl elde edilecektir. GetSoldier metodu içerisinde buna uygun bir kod yer almaktadır. Dikkat edileceği üzere daha önceden havuz içerisinde (Koleksiyon içi), metoda gelen parametre tipinden bir anahtar var ise bunun karşılığı olan nesne anında geri döndürülmektedir. Ancak aksi durumda, söz konusu anahtar (Key) için bir nesne örneklenmekte, koleksiyona (yani havuza) eklenmekte ve geri döndürülmetkedir. Böylece, eklenen bu FlyWeight nesnesinin aynısından tekrar talep edilirse havuzdan karşılanması sağlanmış olacaktır.

Main metodu aslında istemcinin mevzuyu ele aldığı yerdir. soldiers isimli dizi içerisinde SoldierType enum sabitinden pek çok değer tutulmaktadır. Dikkat edileceği üzere tekrar eden bir sürü değer vardır. 3 Private ve 2 Sergeant tipi tanımlanmıştır. SoldierFactory örneklendikten sonra ise tüm bu askerler için ortak bir operasyon gerçekleştirilmektedir. Her biri bulundukları lokasyonlardan farklı bir yere doğru hareket ettirilmektedir. Hareket edilecek yeri istemci belirlemekte ve bunu FlyWeight nesnelerine MoveTo operasyonu yardımıyla bildirmektedir.

Bu noktada for döngüsü içerisinde MoveTo operasyonundan önce (Extrinsic State değerlerinin ele alındığı fonksiyondan önce) fabrika nesnesinden bir Soldier talep edildiğine dikkat edilmelidir. İşte bellek tüketiminin kontrol altına alınmaya başaldığı yer burasıdır. Eğer havuzda bir FlyWeight nesne var ise oradan tedarik edilecek yoksa havuza eklendikten sonra geriye döndürülecektir ki bir sonraki karşılaşmada havuzdan tedarik edilebilsin. Mutlaka farketmişsinizdir bu desen Factory ve Singleton desenelerinide kullanmaktadır. Hatta State ve Strategy nesnelerininde bu kalıp içerisinde ele alındığı görülmektedir.

Bu desen ile ilişkili görsel dersi hazırlayana dek size tavsiyem, istemci tarafındaki for döngüsünde adım adım debug ederek ilerlemeniz olacaktır. Bununla birlikte mutlaka oodesing.com, dofactory.com ve sourcemaking.com/designpatterns sitelerindeki örnekleri incelemenizi öneririm. Böylece geldik bir desenin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[FlyWeightPattern.rar (23,15 kb)](/assets/files/2009/FlyWeightPattern.rar)
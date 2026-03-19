---
title: "Kendi Referans Tiplerimizi Klonlamak"
pubDate: 2005-11-14 10:00:00
categories:
  - C#
tags:
  - C#
---

# Kendi Referans Tiplerimizi Klonlamak
Değerli Okurlarım Merhabalar,

Bu makalemizde kendi yazmış olduğumuz referans tipleri arasında yapılan atama işlemleri sırasında üyeden üyeye (Member by member) kopyalamanın nasıl yapılabileceğini incelemeye çalışacağız. Bildiğiniz gibi referans tipleri belleğin heap bellek bölgesinde tutulurlar. Bu tutuluş yapısının özellikle referans tipleri arasında yapılan atama işlemlerinde önemli bir etkisi vardır. İki referans tipi arasında bir atama işlemi söz konusu olduğunda, aslında bu referans tiplerinin heap bellek bölgesinde yer alan adresleri eşitlenmektedir. Bu eşlemenin doğal bir sonucu olaraktan da referans tiplerinin her hangibirisinde yapılan değişiklik diğerinide otomatikman etkileyecektir.

Ancak bazı durumlarda, özellikle kendi yazdığımız referans tiplerini kullanırken bu durumun tam tersini isteyebiliriz. Yani kendi yazmış olduğumuz bir sınıfın iki nesne örneği arasında yaptığımız atama işlemi sonrası, bu referansların birbirini etkilemelerini istemeyebiliriz. Bu durumda yazmış olduğumuz sınıfa IClonable arayüzünü uygulayarak referans tipinin klonlanmasını sağlayabiliriz. Bu durumu analiz etmeden önce, referans tipleri arasında yapılam atamanın doğal sonucunu aşağıdaki örnek ile incelemeye çalışalım. Örneğimizde bir dörtgene ait kenar uzunluklarını tutacak olan Dortgen isimli bir sınıf kullanacağız. Dortgen sınıfımızın UML şeması ve kodları aşağıdaki gibidir.

![mk140_3.gif](images/mk140_3.gif)

Dortgen.cs

```csharp
public class Dortgen
{
    private double kenarA;
    private double kenarB;

    public double A
    {
        get
        {
            return kenarA;
        }
        set
        {
            kenarA=value;
        }
    }

    public double B
    {
        get
        {
            return kenarB;
        }
        set
        {
            kenarB=value;
        }
    }

    public Dortgen()
    {
        kenarA=0;
        kenarB=0;
    }

    public Dortgen(double aKenari,double bKenari)
    {
        kenarA=aKenari;
        kenarB=bKenari;
    }

    public override string ToString()
    {
        string dortgenBilgi=kenarA.ToString()+" "+kenarB.ToString();
        return dortgenBilgi;
    }
}
```

Şimdi Dortgen sınıfınadan iki nesne örneğini kullanacağımız bir console uygulamsına ait Main metodunda, aşağıdaki kod satırlarını yazalım.

```csharp
Dortgen drtgX=new Dortgen(10,20);
Console.WriteLine("X : "+drtgX.ToString());

Dortgen drtgY;
drtgY=drtgX; // referanslar eşitlenir.
Console.WriteLine("Y : "+drtgY.ToString());

drtgY.A=30;
drtgY.B=40;

Console.WriteLine("Y : "+drtgY.ToString());
Console.WriteLine("X : "+drtgX.ToString());
```

Burada ilk olarak Dortgen sınıfına ait bir nesne örneğini (drtgX) oluşturuyor. Daha sonra ise drtgY isimli bir nesne örneği oluşturup bu örneğe, drtgX nesnesini atıyoruz. İşte bu noktada her iki nesne örneğinin referans adreslerini eşitlemiş oluyoruz. İzleyen satırlarda bu kez drtgY nesnesi üzerinden A ve B özelliklerinin değerlerini değiştiriyoruz. Az önce yapılan eşleştirme işlemi nedeniyle drtgY nesnesi için gerçekleştirilen değişiklikler drtgX nesnesi içinde söz konusu olacaktır. Dolayısıyla drtgX nesnesini ilk örneklerken belirlenen A ve B değişkenlerinin değerlerini kaybetmiş oluyoruz.

![mk140_2.gif](images/mk140_2.gif)

![mk140_6.gif](images/mk140_6.gif)

![dikkat.gif](images/dikkat.gif)
Referans tiplerinin bir birlerine atanması işlemi sonrası, bu tiplerin heap bellek bölgesindeki başlangıç adresleri eşitleneceğinden, birisi içerisindeki varlıklarda yapılacak değişiklikler diğerinede yansıyacaktır.

Yukarıdaki örnek referans tipleri arasında yapılan atamaların bir eksiklik olduğunu göstermez. Bu, bellek sisteminin doğal bir sonucudur. Dahası, referans tiplerinin bu şekilde tutulmasının ve atama işlemleri sonrası adreslerin eşitlenmesinin avantajlı olduğu durumlar da vardır. Örneğin n elemanlı bir diziyi, bir metoda parametre olarak geçirmek istediğinizde, dizinin metod bloğu içinde değer türlerinde olduğu gibi tekrardan örneklenmemesi, referans adreslerinin taşınması sayesinde gerçekleşir. Böylece bellekte gereksiz yere ikinci bir dizi örneği yaratılmamış olunur.

Elbetteki bazı hallerde referans tiplerinin üyeden-üyeye (member by member) kopyalanmasını yani ikinci bir adresleme ile yeni bir örneğin oluşturulmasını isteyebiliriz. İşte bunu gerçekleştirmek için var olan nesneyi atama işlemini yaparken klonlarız. Klonlama işleminin gerçekleştirilebilmesi için, yazmış olduğumuz sınıfa IClonable arayüzünü uygulamamız gerekir. Bu arayüz sadece Clone isimli tek bir metod içermektedir. Yukarıdaki örneğimizde kullandığımız Dortgen sınıfına IClonable arayüzünü aşağıdaki gibi uygulayabiliriz.

![mk140_1.gif](images/mk140_1.gif)

```csharp
public class Dortgen:ICloneable
{
    // Diğer kodlar

    #region ICloneable Members

    public object Clone()
    {
        return new Dortgen(this.kenarA,this.kenarB);
    }

    #endregion
}
```

Elbette Clone metodunu atama işlemi sırasında aşağıdaki kod parçasında olduğu gibi kullanmamız gerekir.

```csharp
drtgY=(Dortgen)drtgX.Clone();
```

Clone metodu, Dortgen sınıfına ait yeni bir nesne örneğini o an sahip olduğu A ve B değerleri ile geriye döndürür. Clone metodu varsayılan olarak Object tipinden değerler döndürdüğü için, drtgY nesnesine yapılan atama işlemi sırasında tür dönüşüm işlemi (casting) yaparak uygun tipe atama yapmamız gerekmektedir. Bu işlem ile birlikte bellekte Dortgen sınıfına ait iki nesne örneği için ayrı ayrı adreslemeler söz konusu olacaktır. drgtX nesnesi var olan değerlerini korurken oluşturulduğu sıradaki adreste konuşlanmaya devam edecektir. drtgY nesnemiz ise drtgX nesnesinin o anki içeriğine sahip olmak üzere, belleğin farklı bir adres bölgesinde yeniden örneklendirilecektir. Durumu aşağıdaki şekil ile daha kolay anlayabiliriz.

![mk140_7.gif](images/mk140_7.gif)

Uygulamayı bu haliyle çalıştırdığımızda atama işlemi sonrası drtgY nesnesi üzerinden yapılan değişikliklerin drtgX nesnesinin değerlerini etkilemediğini görürüz.

![mk140_4.gif](images/mk140_4.gif)

Aynı etkiyi şu şekilde de yapabiliriz,

```csharp
public object Clone()
{
    return this.MemberwiseClone();
}
```

MemberwiseClone metodu Object sınıfına ait bir metoddur. Protected erişim belirleyicisine sahiptir bu yüzden türetme işlemi söz konusu olduğunuda kullanılabilir. Ayrıca override edilebilir bir metod da değildir. MemberwiseClone metodu, kullanıldığı sınıfın nesne örneğinden bir kopya daha oluşturmaktır. Yukarıdaki örneğimizi bu haliyle çalıştırdığımızda, atama sonrası klonlama işleminin başarılı bir şekilde yapıldığını ve drtgY nesnesi üzerinde yapılan değişikliklerin drtgX nesnesini etkilemediğini görürüz.

![mk140_4.gif](images/mk140_4.gif)

Ancak MemberwiseClone metodunu kullanırken dikkat etmemiz gereken bir durum vardır.

![dikkat.gif](images/dikkat.gif)
MemberwiseClone metodu, klonlama işlemi sırasında nesnenin static olmayan tüm değer türlerini (value types) bit-bit kopyalar. Ancak içeride kullanılan referans tipi nesne örnekleri varsa bunların adreslerini aynen yeni örneğe geçirir. Dolayısıyla referans tipleri arası yapılan atama işlemi sonrası oluşan aynı adresi gösterme çatışması, dahili referans tipi nesne örnekleri içinde geçerli olur.

Bu durumda yeni nesne örneğimiz içerisinde var olan bir referans tipi üzerinde yapılacak bir değişiklik, yine ilk nesne içindeki referans nesne örneği içinde geçerli olacaktır. Bu dikkat edilmesi gereken önemli bir durumdur. Örneğin Dortgen sınıfımız içerisinde kullanılacak yeni bir sınıfımız olduğunu varsayalım.

```csharp
public class DortgenHesaplama
{
}

public class Dortgen:ICloneable
{
    private double kenarA;
    private double kenarB;
    private DortgenHesaplama drgH=new DortgenHesaplama();

    public double A
    {
        get { return kenarA; }
        set { kenarA=value; }
    }
    public double B
    {
        get { return kenarB; }
        set { kenarB=value; }
    }
    public DortgenHesaplama Hesaplama
    {
        get { return drgH; }
    }
    public Dortgen()
    {
        kenarA=0;
        kenarB=0;
    }

    public Dortgen(double aKenari,double bKenari)
    {
        kenarA=aKenari;
        kenarB=bKenari;
    }

    public override string ToString()
    {
        string dortgenBilgi=kenarA.ToString()+" "+kenarB.ToString();
        return dortgenBilgi;
    }

    #region ICloneable Members
    
    public object Clone()
    {
        return this.MemberwiseClone();
        // return new Dortgen(this.kenarA,this.kenarB);
    }

    #endregion
}
```

Burada dikkat ederseniz, DortgenHesaplama isimli sınıfa ait bir nesne örneğini Dortgen sınıfı içerisinde kullanmaktayız. Bu, Dortgen referans tipi içerisinde yer alan başka bir referans tipi nesne örneğidir. Atama işlemi sonrası MemberwiseClone metodu DortgenHesaplama sınıfına ait nesne örneğinin sadece adresini kopyalayacaktır. Uygulamamızın kodlarını yukarıdaki Dortgen sınıfına göre aşağıdaki gibi değiştirelim.

```csharp
Dortgen drtgX=new Dortgen(10,20);
Dortgen drtgY;
drtgY=(Dortgen)drtgX.Clone();
drtgY.A=30;
drtgY.B=40;

bool refAdrEsitmi=object.ReferenceEquals(drtgX,drtgY);
Console.WriteLine("drtgX & drtgY referans adresleri eşit mi ?"+refAdrEsitmi.ToString());

refAdrEsitmi=object.ReferenceEquals(drtgX.Hesaplama,drtgY.Hesaplama);
Console.WriteLine("drtgX.Hesaplama & drtgY.Hesaplama referans adresleri eşit mi ?"+refAdrEsitmi.ToString());
```

![mk140_5.gif](images/mk140_5.gif)

![dikkat.gif](images/dikkat.gif)
ReferenceEquals parametre olarak aldığı nesne örneklerinin bellek referanslarının aynı olup olmadığını kontrol ederek sonucu bool tipinden geriye döndüren Object sınıfına ait static bir metoddur.

Görüldüğü gibi drtgX ve drtgY nesnelerini Object sınıfının ReferenceEquals metodu ile karşılaştırdığımızda, false değerini alıyoruz. Çünkü biz Dortgen sınıfımıza IClonable arayüzünü ve Clone metodu içerisinde MemberwiseClone fonksiyonunu uygulayarak, drtgX nesne örneğini klonluyoruz. Bu sebeple heap bellek bölgesinde ayrı bir Dortgen nesne örneği oluşturuluyor. Dolayısıla adresler artık farklı olacağından ReferenceEquals metodu geriye false değer döndürecektir.

Ancak drtgX ve drtgY nesne örnekleri içerisinde yer alan DortgenHesaplama sınıfına ait nesne örneklerinin referanslarını karşılaştırdığımızda true değerinin döndüğünü görmekteyiz. Yani DortgenHesaplama sınıfına ait nesne örnekleri için klonlama işlemi gerçekleşmemiş bunun yerine nesnelerin heap bellek bölgesindeki adresleri eşitlenmiştir. Bu dikkat edilmesi gereken bir durumdur ve sınıflarımızı programlarken gerekli tedbirlerin alınmasını gerektirebilecek kadar önemlidir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

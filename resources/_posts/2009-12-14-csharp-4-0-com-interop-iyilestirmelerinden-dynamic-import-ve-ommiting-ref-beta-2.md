---
layout: post
title: "C# 4.0 - COM Interop İyileştirmelerinden Dynamic Import ve Ommiting Ref [Beta 2]"
date: 2009-12-14 20:30:00 +0300
description: "Bildiğiniz üzere C# 4.0 ile birlikte yine köklü dil değişiklikleri hayatımıza girmiş bulunmakta. Özellikle dinamik diller ile olan etkileşimin arttırılması ve COM dünyası ile olan haberleşmede getirilen yenilikler son derece önemli..."
categories:
  - csharp-4-0
tags:
  - csharp-4-0
  - csharp
  - linq
  - http
  - generics
---
# C# 4.0 - COM Interop İyileştirmelerinden Dynamic Import ve Ommiting Ref [Beta 2]
![blg117_Giris.jpg](/assets/images/2009/blg117_Giris.jpg)

Merhaba Arkadaşlar,

Hani bazen insanın canı şöyle çıtır çıtır kuruyemiş çeker ya...Hatta çoğunlukla bir film seyrederken, maç izlerken, arkadaşları ile sohhet ederken, internette surf yaparken iyi gider ya...Hatta birisinin blog yazısını okurken kuruyemişleri yerken daha bir heyecanlı, istekli olur ya...

![Laughing](/assets/images/2009/smiley-laughing.gif)

İşte bende bu düşünceyle yola çıkıp siz değerli okurlarım kuruyemiş yerken kısa zamanda bir şeyler öğrenebilin, keyifli bir on dakika geçirin diye bu yazıyı hazırladım. Bakalım bu yazımızda bizleri hangi macera bekliyor.

Bildiğiniz üzere C# 4.0 ile birlikte yine köklü dil değişiklikleri hayatımıza girmiş bulunmakta. Özellikle dinamik diller ile olan etkileşimin arttırılması ve COM dünyası ile olan haberleşmede getirilen yenilikler son derece önemli. Bu gelen yenilikler arasında [dynamic](https://www.buraksenyurt.com/post/C-40-Dynamic-Olmak) anahtar kelimesi, [opsiyonal ve isimlendirilmiş parametrelerde (Optional & Named Parameters)](https://www.buraksenyurt.com/post/C-40-Secilebilen-Isimlendirilebilen-Parametreler(Named-and-Optional-Parameters)-ref-i-Gormezden-Gelmek (Ommit-Ref)-ve-PIA-icin-Yenilikler) en çok göze çarpanlar arasında yer almakta. Ancak çok fazla irdelenmeyen fakat özellikle COM Interop dünyasını ilgilendiren minik ve önemli iyileştirmelerde bulunmakta. Bu neden bu kısa yazımızda söz konusu minik iyileştirmelerden ikisini çok basit olarak incelemeye çalışıyor olacağız.

> Kişisel Not: Bu konuda en doğru bilgilere ve güncel örneklere yazının hazırlandığı tarih itibariyle [MSDN](http://msdn.microsoft.com/en-gb/vcsharp/dd819407.aspx)sitesinden ulaşabileceğinizi de hatırlatmak isterim.

Dynamic Import;

Dynamic anahtar kelimesi yardımıyla static olmayan ve COM, dinamik diller gibi ortamlardan gelen nesnelerin ele alınabilmesi mümkün hale gelmektedir. Peki COM Interop ile olan etkileşimde Dynamic tiplerin çaktırmadan geldiğini biliyor muydunuz? Dilerseniz konuyu irdelemek için aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace COMInteropFeatures
{
    class Program
    {
        static void Main(string[] args)
        {
            var excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            excelApp.Visible = true;

            #region DynamicImport özelliği olmadan önce

            ((Excel.Range)excelApp.Cells[1, 1]).Value2 = "ID";

            Excel.Range range12 = (Excel.Range)excelApp.Cells[1, 2];
            range12.Value2 = "Phone Number";

            #endregion
        }
    }
}
```

Bu örnekte Excel API'sine ulaşılaraktan bir Workbook oluşturulması ve aşağıdaki görüntünün üretilmesi sağlanmaktadır.

![blg117_Excel.gif](/assets/images/2009/blg117_Excel.gif)

Üzerinde önemle durmamız gereken nokta ise 1,1 ile 1,2 koordinatlarındaki hücrelere veriyi nasıl yazdığımızdır. Dikkat edilecek olursa 1,1 hücresine yazı yazmak için Range tipine bir dönüştürme işlemi yapılmıştır. Bu dönüşüm işlemi sonrasında Value2 özelliğine ulaşılabilmiştir. Devam eden kod satırında ise önce Range tipine dönüştürme ve atama işlemi yapılmış, sonrasında Value2 özelliğine gidilmiştir. Bu açıdan bakıldığında bir dönüştürme işlemi yapılmasının kaçınılmaz olduğu görülmektedir. Elbette bu eskiden böyleydi. Artık dynamic tipinin COM Interop nesneleri içerisine serpiştirildiğini görmekteyiz. Aşağıdaki görüntü bu durumu son derece iyi açıklamaktadır.

![blg117_DynamicImport.gif](/assets/images/2009/blg117_DynamicImport.gif)

Görüldüğü gibi Cells üzerinden ulaşılan tip dynamic olarak ele alınmaktadır. Buna göre yukarıdaki kod parçası dynamic import kabiliyeti sayesinde dönüştürme işlemlerine gerek duyulmadan aşağıdaki gibi yazılabilir.

```csharp
excelApp.Cells[1, 1].Value = "ID";

Excel.Range range12_ = excelApp.Cells[1, 2]; // Cast yapılmadan doğrudan atama işlemi
range12_.Value2 = "Phone Number";
```

Sonuç aynı olacaktır. Dikkat edilmesi gereken nokta herhangibir cast işlemi yapılmasına gerek olmayışıdır. Diğer yandan dynamic olan tiplerin çalışma zamanında çözümlenmesi söz konusu olduğundan aşağıdaki durumda bir handikap olarak görülebilir.

![blg117_Handikap.gif](/assets/images/2009/blg117_Handikap.gif)

Cells[1,2]. sonrasında kullanabileceğimiz metodları gören, bilen, hatırlayan var mı?

![Undecided](/assets/images/2009/smiley-undecided.gif)

Omitting Ref;

Yine COM Interop nesneleri ile olan münasibetlerimizde yaşadığımız sorunlardan biriside ref tipinden parametrelerin aktarılması için mutlaka geçici de olsa değişken tanımlamaları yapılması gerekliliğidir. Durumu daha net anlayabilmek için aşağıdaki kod parçasını göz önüne alalım.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word = Microsoft.Office.Interop.Word;

namespace COMInteropFeatures
{
    class Program
    {
        static void Main(string[] args)
        {
            #region omitting ref

            #region öncesi

            Word.Application wordApp = new Word.Application();
            wordApp.Visible = true;
            object filePath = Environment.CurrentDirectory+"\\Belge.docx";
            object missing = Type.Missing;

            wordApp.Documents.Open(ref filePath, ref missing, ref missing, ref missing, ref missing, ref missing,  ref missing, ref missing, ref missing, ref missing, ref missing,  ref missing, ref missing, ref missing, ref missing, ref missing);

            #endregion

            #endregion
        }
    }
}
```

Bu seferki örneğimizde çok basit olarak Word Interop nesnesini kullanarak Belge.docx isimli dosyanın açılması sağlanmaktadır. Ancak Open metodunun yazılışı mutlaka dikkatinizi çekmiştir. Peki bir sürü ref missing yazmamış dışında bir sıkıntı görebiliyor musnuz?

![Wink](/assets/images/2009/smiley-wink.gif)

Aslında Named ve Optional Parametre özellikleri ile bu kod stilinden zaten kurtulduk. Ne varki buradaki sıkınta bu değil. Sıkıntı, ref tipinden olan parametreler için missing isimli object tipinden bir değişken tanımlamak zorunda olmamız.

Bu birden fazla çeşitte ref parametresi alan bir COM Interop çağrısı için birden fazla geçici değişken tanımlamak zorunda kalabiliriz anlamına da gelmekte. İşte C# 4.0 ile gelen Omitting Ref (ref'leri göz ardı etmek olarak düşünebiliriz) kabiliyeti sayesinde artık ref olarak kullanılması gereken parametrelere değer türü (Value Type) şeklinde argüman geçirebilmekteyiz. Peki ref kullanımından kaçılıyor mu? Elbetteki hayır. Arka planda derleyici bizim için gerekli geçici değişkenleri zaten oluşturuyor ve metod yine referans tipinden gelen parametreler ile çalışıyor. Kısacası yukarıdaki kodu aşağıdaki şekilde yazmamız mümkün.

```csharp
wordApp.Documents.Open(filePath, Type.Missing, Type.Missing);
```

Dikkat edileceği üzere doğrudan değer ataması yapılmış, herhangibir değişken kullanımına gidilmemiştir.

İşte sizlere bir kaç dakika içerisinde çerez niyetine okuyup öğrenebileceğiniz bir yazı. Umarım faydalı olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
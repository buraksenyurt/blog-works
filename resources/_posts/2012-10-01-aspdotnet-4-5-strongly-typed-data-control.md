---
layout: post
title: "Asp.Net 4.5- Strongly Typed Data Control"
date: 2012-10-01 02:45:00 +0300
description: "Malumunuz Web tarafı ile aram pek iyi değildir. Ancak .Net Framework’ ün her sürümünde genel olarak gelen yeniliklere bakmaya çalışıyorum/çalışmaktayım. Geçtiğimiz hafta içerisinde de Asp.Net 4.5 tarafında gelen yenilikleri incelemeye başladım. Bunlar arasında dikkatimi çekenlerden birisi de, Web Form’ larda veri bağlı kontroller(Data Bind Controls) için gelen strongly typed ve intelli-sense desteğiydi. Durumu daha iyi aktarabilmem için basit bir örnek üzerinden ilerlemeye çalışım. İlk etapta aşağıdaki gibi bir POCO(Plain OLD CLR object) tipimiz olduğunu düşünelim."
categories:
  - aspnet-4-5
tags:
  - aspnet-4-5
  - csharp
  - xml
  - dotnet
  - aspnet
  - xaml
  - http
  - reflection
  - generics
---
# Asp.Net 4.5- Strongly Typed Data Control
[![strong](/assets/images/2012/strong_thumb.gif)](/assets/images/2012/strong.gif) Merhaba Arkadaşlar,

Malumunuz Web tarafı ile aram pek iyi değildir. Ancak.Net Framework’ ün her sürümünde genel olarak gelen yeniliklere bakmaya çalışıyorum/çalışmaktayım. Geçtiğimiz hafta içerisinde de Asp.Net 4.5 tarafında gelen yenilikleri incelemeye başladım.

Bunlar arasında dikkatimi çekenlerden birisi de, Web Form’ larda veri bağlı kontroller (Data Bind Controls) için gelen strongly typed ve intelli-sense desteğiydi. Durumu daha iyi aktarabilmem için basit bir örnek üzerinden ilerlemeye çalışım. İlk etapta aşağıdaki gibi bir POCO (Plain OLD CLR object) tipimiz olduğunu düşünelim.

(Bu arada yandaki halter kaldıran adam ne alak diyebilirsiniz. Giriş yazısını düşünürken, Strongly kelimesinden Strong ifadesine gelince, bunu anlatabilecek fotoğraflardan birisi olarak karşıma çıktı ![Confused smile](/assets/images/2012/wlEmoticon-confusedsmile_25.png))

[![stdi_1](/assets/images/2012/stdi_1_thumb.png)](/assets/images/2012/stdi_1.png)

```csharp
namespace STDC_Old 
{ 
    public class Player 
    { 
        public int PlayerId { get; set; } 
        public string Nickname { get; set; } 
        public int Score { get; set; } 
    } 
}
```

Senaryomuzda bu basit POCO tipine ait nesne örneklerinden oluşan generic bir koleksiyonu, sayfa üzerindeki bir FormView kontrolüne bağlıyor olacağız (senaryoda örnek olarak FormView kontrolünü göz önüne aldık. Pek tabi diğer data bind bileşenleri de örneğe katabilirsiniz) Web uygulamamıza ait arka plan kodlarını aşağıdaki gibi tasarlayabiliriz. (Projemiz Asp.Net Empty Web Application tipindedir ve.Net Framework 4.0 hedefli olarak oluşturulmuştur)

```csharp
using System; 
using System.Collections.Generic; 
using System.Web.UI;

namespace STDC_Old 
{ 
    public partial class Default 
        : System.Web.UI.Page 
    {        
        static List<Player> players = new List<Player> 
        { 
            new Player{ PlayerId=1, Nickname="Brit", Score=125}, 
            new Player{ PlayerId=2, Nickname="Kuşbeyin", Score=250}, 
            new Player{ PlayerId=3, Nickname="Zeyno", Score=90}, 
            new Player{ PlayerId=4, Nickname="Nikol", Score=175} 
        }; 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            if (!Page.IsPostBack) 
            { 
                formviewPlayers.DataSource = players; 
                formviewPlayers.DataBind(); 
            } 
        } 
    } 
}
```

Kod parçasında görüldüğü üzere formviewPlayers isimli FormView kontrolüne players isimli bir koleksiyon içeriği bağlanmaktadır. Peki ya tasarım ortamında durum nedir? Tahmin edileceğiz üzere çalışma zamanında, bir Player nesne örneğinin PlayerId, Nickname ve Score gibi özelliklerinin değerlerini tek (one way) veya çift yönlü (two way) olacak şekilde göstermek için Bind veya Eval tiplerinden yararlanılmaktadır. Bu tipleri genellikle Item Template elementleri içerisindeki kontrollerde sıklıkla kullanırız. Söz gelimi aşağıdaki gibi

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_126.png)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="STDC_Old.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
        <asp:FormView runat="server" ID="formviewPlayers" DefaultMode="Edit"> 
        <EditItemTemplate> 
                   Player Id 
                   <asp:Label ID="lblPlayerId" runat="server" Text='<%#Bind("PlayerId") %>'/> 
                   <br /> 
                   Nick name 
                   <asp:TextBox ID="txtNickname" runat="server" Text='<%#Bind("Nickname") %>' /> 
                   <br /> 
                   Score 
                   <asp:TextBox ID="txtScore" runat="server" Text='<%#Bind("Score") %>' /> 
                   <br /> 
           <asp:Button ID="btnUpdate" Text="Update Player Informations" runat="server" CommandName="Update"/> 
            <br /> 
        </EditItemTemplate> 
</asp:FormView> 
    </div> 
    </form> 
</body> 
</html>
```

Dikkat edileceği üzere lblPlayerId, txtNickname, txtScore isimli bileşenlerin Text nitelikleri, Player tipinin sırasıyla PlayerId, Nickname ve Score özelliklerine (Properties) bağlanmışlardır. FormView bileşeninde, Bind tipi kullanılmış ve çift yönlü veri bağlama imkanı sunulmuştur. Ama bildiğiniz üzere Eval de kullanılabilir ve tek yönlü bir veri akışı da sağlanabilir. Kaldı ki hangisini kullandığımızı şu anda pek bir önemi yok

![Sarcastic smile](/assets/images/2012/wlEmoticon-sarcasticsmile_7.png)

Peki buradaki kodları yazarken hiç şöyle bir şey de olsun ister miydiniz?

> Bind veya Eval kullanırken keşke kontrolü bağladığımız tipin özelliklerini de görebilsekde, neyi neye bağladığımızı daha kolay takip edebilsek. Hatta burada Intelli-sense kabiliyetleri de bize yardımcı olsa
>
> ![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_126.png)

Çünkü yukarıdaki kod parçasını yazarken Eval veya Bind kullanımlarında tamamen ezbere olacak şekilde string veriler yazılmaktadır. Bu özellikle kalabalık veri toplulukları göz önüne alındığında dikkat dağıtıcı ve zorlayıcı bir durumdur. Ayrıca hata yapma riski vardır. İfadeler yanlış yazılabilir ve sonuçlar ancak çalışma zamanında görülecektir.

Diğer yandan arka plandaki çalışma sistematiğine baktığımızda, bağlanan özelliğin elde edilebilmesi için string içeriğin çözümlenmesi gerektiği de aşikardır. Bu da çok doğal olarak reflection tabanlı bir yaklaşımı gerektirmektedir. Bu arada yukarıdaki kodun ekran çıktısı aşağıdaki gibi olacaktır onu da ifade edeyim.

[![stdi_2](/assets/images/2012/stdi_2_thumb.png)](/assets/images/2012/stdi_2.png)

Gelelim Asp.Net 4.5 tarafındaki duruma. Yine aynı örnek senaryoyu ele alacağız ancak bu kez veri bağlı kontrolümüzün ItemType isimli özelliğine, bağlanacak olan tipin (kuvvetle muhtemel bir sınıf adı olacaktır) adını vereceğiz. Aşağıdaki ekran görüntüsünde olduğu gibi. Üstelik intelli-sense desteğimiz de var

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_126.png)

[![stdi_3](/assets/images/2012/stdi_3_thumb.png)](/assets/images/2012/stdi_3.png)

Volaaaa! demek için aslında çok erken. Nitekim iş asıl tipi belirttikten sonra güzelleşiyor. ItemType tanımlaması ile söz konusu kontrolün veri bağlı içeriğinin (bir başka deyişle child element olacak kontrollerin) hangi.Net tipini değerlendirmesi gerektiğini ifade etmiş oluyoruz. İşte bu noktadan sonra sunucu bazlı kontrollerin ilgili özelliklerine veri bağlama işlemlerini yaparken intelli-sense özelliğinden yararlanabiliriz. Artık elimizde Strongly Typed bir veri bağlı kontrol bulunmakta. Aşağıdaki ekran görüntüsünde bu durum ve kullanım kolaylığı daha net bir şekilde görülebilir.

[![stdi_4](/assets/images/2012/stdi_4_thumb.png)](/assets/images/2012/stdi_4.png)

Sanırım şimdi volaaaaaa!!! diyebiliriz

![Smile](/assets/images/2012/wlEmoticon-smile_58.png)

> Buradaki yaklaşım XAML tarafından size tanıdık gelecektir. XAML tarafında da asıl kontrolün bir veri kaynağına bağlanması (yani bir Resource bağlanması), iç elementlerin özelliklerinin de bu verinin niteliklerine atanması işlemi, dekleratif olarak yapılmaktadır.

Bu durumda Asp.Net 4.5 versiyonu ile senaryomuzun aspx içeriği aşağıdaki gibi olacaktır.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="STDC_New.Default" %>

<!DOCTYPE html> 
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
                <br /> 
        <asp:FormView runat="server" ID="formviewPlayers" DefaultMode="Edit" ItemType="STDC_New.Player"> 
        <EditItemTemplate> 
                   Player Id 
                   <asp:Label ID="lblPlayerId" runat="server" Text='<%#BindItem.PlayerId %>'/> 
                   <br /> 
                   Nick name 
                   <asp:TextBox ID="txtNickname" runat="server" Text='<%#BindItem.Nickname %>' /> 
                   <br /> 
                   Score 
                   <asp:TextBox ID="txtScore" runat="server" Text='<%#BindItem.Score %>' /> 
                   <br /> 
           <asp:Button ID="btnUpdate" Text="Update Player Informations" runat="server" CommandName="Update"/> 
        </EditItemTemplate> 
</asp:FormView> 
    </div> 
    </form> 
</body> 
</html>
```

Tabi bu esnekliği kullanırken aklınıza “acaba noktaya basıp özellik adını yazdıktan sonra gelen fonksiyonellikleri de kullanabilir miyiz?” diye bir soru gelebilir

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_126.png)

Söz gelimi “string tipinden olan Nickname’ i bağladıktan sonra bir de ToUpper metodunu çağırsak da, ismi büyük harf yazsa olmaz mı?” diyebilirsiniz. Ben bunu denediğimde aşağıdaki çalışma zamanı hatasını aldım.

[![stdi_5](/assets/images/2012/stdi_5_thumb.png)](/assets/images/2012/stdi_5.png)

Bu gibi durumlara dikkat etmek gerektiğini ifade edebiliriz.

Şimdi senaryomuzu biraz daha ilginçleştirelim

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_126.png)

Bu sefer özelliklerinden birisi veri kontrolüne bağlanabilir liste koleksiyonu tipinden olan bir sınıfı ele alıp çalışma zamanı içeriğini master-detail formatında göstermeye çalışıyor olacağız. Senaryomuzda yine yeni gelen ItemType ve BindItem özelliklerine yer vereceğiz. İlk etapta uygulamamıza Game isimli yeni bir POCO tipi eklediğimizi düşünelim. Aşağıdaki sınıf diyagramında görüldüğü gibi.

[![stdi_7](/assets/images/2012/stdi_7_thumb.png)](/assets/images/2012/stdi_7.png)

Dikkat edileceği üzere Game tipi içerisinde Player sınıfına ait örnekleri taşıyacak List tipinden generic bir özellik de bulunmaktadır. Dolayısıyla bir Game alanındaki n sayıda Player’ ı taşıyabiliriz. Default.aspx.cs içeriğini de aşağıdaki gibi kodlayabiliriz.

```csharp
using System; 
using System.Collections.Generic; 
using System.Web.UI;

namespace STDC_New 
{ 
    public partial class Default 
        : System.Web.UI.Page 
    { 
        static List<Player> players1 = new List<Player> 
        { 
            new Player{ PlayerId=1, Nickname="Brit", Score=125}, 
            new Player{ PlayerId=2, Nickname="Kuşbeyin", Score=250}, 
            new Player{ PlayerId=3, Nickname="Zeyno", Score=90}, 
            new Player{ PlayerId=4, Nickname="Nikol", Score=175} 
        };

        static List<Player> players2 = new List<Player> 
        { 
            new Player{ PlayerId=6, Nickname="Legolat", Score=450}, 
            new Player{ PlayerId=7, Nickname="Aragorn", Score=485}, 
            new Player{ PlayerId=8, Nickname="Gandalf the Gray", Score=555}, 
            new Player{ PlayerId=9, Nickname="Arven", Score=1005} 
        };

        static List<Game> mordor = new List<Game> 
        { 
            new Game{ 
             GameId=1001, 
             Title="Mordor Game Zone", 
             Players=players1 
            }, 
            new Game{ 
             GameId=4587, 
             Title="Gondor Game Zone", 
             Players=players2 
            } 
        };

        protected void Page_Load(object sender, EventArgs e) 
        { 
            if (!Page.IsPostBack) 
            { 
                listViewGame.DataSource = mordor; 
                listViewGame.DataBind(); 
            } 
        } 
    } 
}
```

Bizim için önemli olan nokta listViewGame isimli ListView kontrolünün içeriğidir. Bu içeriği aşağıdaki gibi ürettiğimizi düşünelim.

```xml
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="STDC_New.Default" %>

<!DOCTYPE html> 
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
        <div> 
            <asp:ListView runat="server" ID="listViewGame" 
               ItemType="STDC_New.Game"> 
                <ItemTemplate> 
                    <div style="background-color: gold"> 
                        <asp:Label ID="lblGameIt" runat="server" 
                           Text="<%#BindItem.GameId %>" /> 
                        Title : 
                        <asp:Label ID="lblGameTitle" runat="server" 
                           Text="<%#BindItem.Title %>" /><br /> 
                    </div> 
                    <div style="border: medium; border-color: black; background-color: lightgray"> 
                        <asp:ListView ID="listViewPlayers" 
                            DataSource="<%#BindItem.Players %>" 
                            ItemType="STDC_New.Player" runat="server"> 
                            <ItemTemplate> 
                                <div style="border: double; border-color: red"> 
                                    <asp:Label ID="lblPlayerId" runat="server" 
                                       Text='<%#BindItem.PlayerId %>' /><br /> 
                                    Nickname        : 
                                    <asp:Label ID="txtNickname" runat="server" 
                                        Text='<%#BindItem.Nickname %>' /><br /> 
                                    Current Score   : 
                                    <asp:Label ID="txtScore" runat="server" 
                                        Text='<%#BindItem.Score %>' /><br /> 
                                </div> 
                            </ItemTemplate> 
                        </asp:ListView> 
                    </div> 
                </ItemTemplate> 
            </asp:ListView> 
        </div> 
    </form> 
</body> 
</html>
```

İlk olarak en dıştaki ListView kontrolünün ItemType özelliğine STDCNew.Game değerini atadığımızı görmekteyiz. Buna göre listViewGame kontrolünün alt elementlerindeki bileşenlere ait özelliklerde, Game sınıfının özelliklerini kullanabiliriz. lblGameId ve lblGameTitle isimli Label kontrollerindeki Text özelliklerini, Game tipinin sırasıyla GameId ve Title özelliklerine bağlıyoruz.

Yine alt elementlerden birisi olarak iç kısımda yer alan listViewPlayers isimli ListView bileşeninde ise, daha farklı bir veri bağlama işlemi yapıldığı hemen gözünüze çarpmış olmalıdır. Dikkat edileceği üzere DataSource elementine BindItem.Players şeklinde bir atama yapılmıştır. Bir başka deyişle çalışma zamanında üst tarafa bağlı olan Game nesne örneğinin içerisindeki Players özelliğinin işaret ettiği koleksiyonun veri kaynağı olarak kullanılacağı belirtilmektedir.

> Eğer DataSource özelliğine bu şekilde bir atama işlemi yapılmassa, Game örneklerinin Players koleksiyonlarına ait içerikleri ekrana basılmayacaktır. Böyle bir durumda aşağıdakine benzer bir ekran çıktısı elde edilir.
> [![stdi_10](/assets/images/2012/stdi_10_thumb.png)](/assets/images/2012/stdi_10.png)

Ayrıca listViewPlayer ListView kontrolünün ItemType özelliğine de SDTCNew.Player değeri verilmiştir. Buna göre alt elementlerdeki kontrollerin özelliklerinde Player sınıfının özellikleri kullanılabilecektir.

> Pek tabi kodu yazarken intelli-sense özelliği de devreye girmekte ve aşağıdaki ekran çıktılarında olduğu gibi bizlere kolaylık sağlamaktadır.
> [![stdi_8](/assets/images/2012/stdi_8_thumb.png)](/assets/images/2012/stdi_8.png) [![stdi_9](/assets/images/2012/stdi_9_thumb.png)](/assets/images/2012/stdi_9.png)

Örneği çalıştırdığımızda çalışma zamanında aşağıdaki ekran görüntüsünde yer alan sonuçları aldığımızı görürüz.

[![stdi_6](/assets/images/2012/stdi_6_thumb.png)](/assets/images/2012/stdi_6.png)

Görüldüğü gibi iç içe iki ListView bileşeni Master-Detail formasyonda oyun sahalarını ve bu sahalardaki oyuncuları gösterecek şekilde üretilebilmiştir. Böylece geldik bir yazımızın daha sonuna. Bu kısa çerez tadındaki yazı ile Asp.Net 4.5 ile gelen yeni kabiliyetlerden birisine değinmiş olduk. Bir başka yazımızda görüşmek dileğiyle, hepinize mutlu günler dilerim.

[ASPNET45_NewFeatures.zip (44,72 kb)](/assets/files/2012/ASPNET45_NewFeatures.zip)
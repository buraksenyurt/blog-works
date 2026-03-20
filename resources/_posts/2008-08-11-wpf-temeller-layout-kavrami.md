---
layout: post
title: "WPF Temeller : Layout Kavramı"
date: 2008-08-11 03:00:00 +0300
categories:
  - wpf
tags:
  - wpf
  - xml
  - xaml
  - http
  - visual-studio
---
Uzun süredir ara verdiğimiz makalelerimize kaldığımız yerden devam ediyoruz. Bu makalemizde çok basit seviyede Windows Presentation Foundation uygulamalarının temellerinden birisi olan Layout kavramını inceleme çalışıyor olacağız. WPF uygulamalarında kullanılan ekranlara ait element veya kontrollerin mutlaka bir Layout bileşeni içerisinde konuşlandırılmış olmaları gerekmektedir. Layout bileşenleri temelde birer Panel olarak düşünülmelidir.

Bu açıdan bakıldığında klasik Windows programcılığında yer alan (diğer kontrolleri üzerinden taşıyan) Container bileşenlerinede benzetilebilirler. WPF uygulamalarında kullanılabilecek olan 6 adet temel Layout bileşeni bulunmaktadır. Herbirinin kendine özgü şekilde elementleri gösterme ve yerleştirme seçenekleri vardır. Bu bileşenlerin en önemli ortak özelliği ise Panel isimli abstract sınıftan (Class) türemiş (Inherit) olmalarıdır. Aşağıdaki sınıf şemasında (Class Diagram) söz konusu bileşenler ve Panel ile aralarında türetimsel ilişki açık bir şekilde görülebilmektedir.

> Layout bileşenlerinin tamamı, System.Windows.Controls isim alanı (namespace) altında yer almaktadır. Bu isim alanı ise, WPF Managed API katmanında yer alan PresentationFramework.dll assembly'ının bir parçasıdır. Bu assembly içerisinde Layout bileşeni dışında üst seviye kontrolleri, style'ler vb... bileşenlerde yer almaktadır. Bilindiği üzere WPF mimarisinde Managed API katmanında PresentationCore.dll ve WindowsBase.dll assembly'larıda yer almaktadır.

![mk257_1.gif](/assets/images/2008/mk257_1.gif)

Canvas bileşeni, üzerine bırakılan elementlerin pencerenin sol (Left), sağ (Right), üst (Top) ve alt (Bottom) eksenlerine olan uzaklıklarına göre bir yerleşim planına imkan tanımaktadır. DockPanel bileşeni, elementlerin tüm alanı kaplayacak şekilde sola, sağa, üste, alta ve geri kalan boşluklara (Fill) yanaştırılarak yerleştirilmelerine izin vermektedir. StackPanel bileşeni varsayılan olarak elementleri alt alta dizen bir yerleşim sunmaktadır. Ancak istenirse elementleri yatay eksende (Horizontal) yan yana olacak biçimde yerleştirilebilmelerine de izin vermektedir. WrapPanel bileşeni, dikey veya yatay düzlemde elemanları birbirlerine bitiştirerek sıralarken ekranın sonlanması gibi durumları otomatik hesap edip gerekli kaydırmaların yapılmasına olanak tanımaktadır (Karışık gelen bu tasvir, ilerleyen kısımlardaki kodlar ile daha net bir şekilde görülebilecektir). Grid bileşeni, hücreleri, satır ve sütunları kullanarak kontrollerin yerleşimlerinin gerçekleştirilmesini sağlamaktadır. Son olarak UniformGrid kontrolü ise sabit boyuttaki hücreleri kullanarak bileşenlerin çok kolay ve hızlı bir biçimde yerleştirilebilmelerini sağlamaktadır.

> Dikkat edileceği üzere tüm Layout bileşenleri Abstract Panel sınıfından türemektedir. Buda, geliştirici tanımlı taşıyıcı Layout kontrollerinin yazılabileceği anlamına gelmektedir. Bilindiği üzere abstract sınıflar, kendisinden türeyen tiplerin uyması ve ezmesi şart olan üye bildirimlerini içermekte olup doğrudan örneklenerek kullanılamayan tiplerdir. Ayrıca abstract sınıflar polimorfik şekilde davranış gösterebilirler. Buda Plug-In tabanlı mimarilerde önem arz eden bir konudur.

Bu kısa teorik bilgilerden sonra örnekler üzerinden ilerleyerek Layout bileşenlerini kavramakta yarar olacağı kanısındayım.

WrapPanel;

İlk olarak WrapPanel ile başlayalım. Örnek bir WPF uygulamasında yer alan Window sınıfına ait XAML (eXtensible Application Markup Language) içeriğini aşağıdaki gibi geliştirdiğimizi düşünelim. (Makalede yer alan örnekler Visual Studio 2008 Professional üzerinde geliştirilmektedir.)

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <WrapPanel Orientation="Horizontal">
        <Button Name="Button1" Content="Giriş"/>
        <Button Name="Button2" Content="Kaydet"/>
        <Button Name="Button3" Content="Hesapla"/>
        <Button Name="Button4" Content="Kullanıcı Değiştir"/>
        <Button Name="Button5" Content="Detayları Al"/>
        <Button Name="Button6" Content="Yükle"/>
        <Button Name="Button7" Content="Belleği Sil"/>
    </WrapPanel>
</Window>
```

Söz konusu Window1 penceresinin tasarım zamanındaki ekran görüntüsü aşağıdaki gibi olacaktır.

![mk257_2.gif](/assets/images/2008/mk257_2.gif)

Dikkat edileceği üzere WrapPanel içerisinde yer alan örnek Button kontrolleri, ekranın boyutuna göre otomatik olarak aşağıya kaydırılmaktadır. WrapPanel bileşenine ait Orientation özelliğinin varsayılan değeri Horizontal'dır. Bu sebepten açık bir şekilde belirtilmesine gerek yoktur. Ancak elementlerin (kontrollerin) dikey düzlemde kaydırılmasını istiyorsak, Orientation özelliğine Vertical değerinin verilmesi gerekir. Vertical değeri set edildikten sonra tasarım zamanında aşağıdaki sonuç elde edilir.

![mk257_3.gif](/assets/images/2008/mk257_3.gif)

Elbetteki çalışma zamanında ekran boyutları ile oynanılması halinde düğmelerin yerleşimleride buna göre değişiklik gösterecektir. Örneğin boyut ile çalışma zamanında oynandıktan sonraki olası hal aşağıdaki ekran görüntüsüne benzer olacak şekilde elde edilebilir.

![mk257_4.gif](/assets/images/2008/mk257_4.gif)

DockPanel;

Windows ile programlama yapan herkes özellikle Visual Studio ortamında kontrollerin taşıyıcılar (Container) içerisindeki yerlerini belirlemede kullanılan Dock özelliğini (Property) bilir. DockPanel bu yaklaşımı uygulacak şekilde çalışmaktadır. İşte buna örnek olacak bir XAML çıktısı.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <DockPanel>
        <Button DockPanel.Dock="Top">Üst Taraf</Button>
        <Button DockPanel.Dock="Left">Sol Taraf........</Button>
        <Button DockPanel.Dock="Right">Sağ Taraf</Button>
        <Button DockPanel.Dock="Bottom">Alt Taraf</Button>
        <Button>Kalan Kısımlar</Button>
    </DockPanel>
</Window>
```

Burada en önemli nokta iliştirilmiş özellik (Attached Property) kullanılarak ilgili elementin DockPanel taşıyıcısının hangi bölgesine yanaştırılacağının belirlenmesidir. Söz gelimi DockPanel.Dock özelliğine Top değeri verilmesi ile Button elementinin DockPanel bileşeninin üst tarafına yanaştırılacağı belirtilmektedir. İlginç olan noktalardan biriside son Button kontrolü için böyle bir özellik tanımlaması yapılmamış olmasıdır. Bu çok doğal olarak kalan kısmı dolduracak bir kontrol yerleşimine neden olmaktadır. Window1' in tasarım zamanındaki görüntüsü aşağıdaki gibi olacaktır.

![mk257_5.gif](/assets/images/2008/mk257_5.gif)

StackPanel;

StackPanel bileşeni makalenin başındada değinildiği gibi içerisindeki elementleri eklendiği sıra ile yatay veya dikey düzlemde dizerek göstermektedir. Varsayılan olarak tüm bileşeneleri yukarıdan aşağıdaki doğru dizmektedir. Hemen aşağıdaki XAML içeriğini göz önüne alalım.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <StackPanel Background="Gold">
        <Button x:Name="Button1" Content="Button1" Margin="3,5" HorizontalAlignment="Left" />
        <CheckBox Content="Çalışıyor mu?" IsChecked="True"/>
        <TextBlock Text="Yaşadığı Şehir" VerticalAlignment="Bottom" HorizontalAlignment="Center"/>
        <ComboBox>
            <ComboBoxItem Content="İstanbul" Foreground="Brown"/>
            <ComboBoxItem Content="İzmir" Foreground="Red"/>
            <ComboBoxItem Content="Ankara" Foreground="Blue"/>
            <ComboBoxItem Content="Antalya" Foreground="Goldenrod"/>
        </ComboBox>
        <Button Content="BilgleriOnayla" x:Name="Button2"/>
    </StackPanel>
</Window>
```

Bu içerikte dikkat edileceği üzere Button, CheckBox, TextBlock, ComboBox gibi değişik tipte bileşenler kullanılmaktadır. Normal şartlarda StackPanel içerisindeki tüm bileşenler kullanabildikleri tüm alanı kaplarlar. Bu sebepten ComboBox ve BilgileriOnayla başlıklı Button kontrolünün yatayda tüm alanı kapladıkları görülür. Ancak burada VerticalAlignment, HorizontalAlignment, Width, Height, Margin gibi özellikler ile oynanarak, kontrollerin boyutları ve StackPanel içerisinde kaplayacakları alanlar değiştirilebilir. Söz gelimi Button1 isimli düğmede Margin değeri 3,5 olarak verilmiştir. Yani sol üst (Left,Top) kenar uzaklıkları 3 ile 5 piksel olarak belirlenmiştir. Buna ek olarak HorizontalAlignment değerinin Left verilmesi ile Button kontrolünün sol tarafa yakın çıkması ama üst taraftan 5, sol taraftan ise 3 piksel uzaklıkta durması sağlanmıştır. Benzer bir konumlandırma işlemide TextBlock bileşeni üzerinden VerticalAlignment ve HorizontalAlignment özelliklerine ilgili değerler atanarak gerçekleştirilmektedir. Bunlara göre tasarım zamanındaki ekran görüntüsü aşağıdaki gibi olacaktır.

![mk257_6.gif](/assets/images/2008/mk257_6.gif)

Eğer StackPanel bileşeninin Orientation özelliğine Horizontal değeri atanırsa sonuç aşağıdaki gibi olacaktır.

![mk257_7.gif](/assets/images/2008/mk257_7.gif)

Bu kez görüleceği üzere tüm elementler yan yana dizilmektedir. Elbetteki yerleşimler biraz tuhaflaşmıştır ve bunların ilgili özellikler yardımıyla düzenlenmesi gereklidir.

UniformGrid;

Belkide Grid tipinden taşıyıcılardan en kolay kullanıma sahip olanıdır. Nitekim geliştiricinin hücreleri (Cell), satır (Row) veya sütun (Column) özelliklerini düşünmesine gerek yoktur. UniformGrid bileşeninde içeriye eklenen elementlere göre hücrelerin boyutları, satır ve sütun sayıları sabitlenmektedir. Söz gelimi aşağıdaki XAML içeriğini göz önüne alalım.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <UniformGrid>
        <Button>1</Button>
        <Button>2</Button>
        <Button>3</Button>
        <Button>4</Button>
        <Button>5</Button>
        <Button>6</Button>
        <Button>7</Button>
        <Button>8</Button>
    </UniformGrid>
</Window>
```

UniformGrid bileşeni içerisine 8 adet Button kontrolü eklenmiştir. Bu satır ve sütun sayıları otomatik olarak belirlendiğine göre 3X3' lük bir ızgara anlamına gelmektedir. Malum son hücre boş kalacaktır. İşte örnek XAML içeriğine ait tasarım zamanı görüntüsü;

![mk257_8.gif](/assets/images/2008/mk257_8.gif)

Ancak tabikide kolon (Column) veya satır (Row) sayıları ile oynanabilir yada içerideki elementlerin hücre içerisinde bulundukları konumlar değiştirilebilir. Bu durumu daha iyi analiz etmek için aşağıdaki XAML içeriği göz önüne alınabilir.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <UniformGrid Columns="4">
        <Button Margin="5">1</Button>
        <Button Background="RosyBrown" Margin="10,15">2</Button>
        <Button>X</Button>
        <Button>4</Button>
        <Button>5</Button>
        <Button>6</Button>
        <Button>7</Button>
        <Button Foreground="Gold" Background="Black" BorderBrush="Brown" BorderThickness="2" Margin="7.5">8</Button>
    </UniformGrid>
</Window>
```

Burada görüldüğü gibi UniformGrid bileşeninin, içerisindeki kontrolleri 4 sütundan oluşan bir ızgara içerisinde göstereceği Columns isimli özelliğe atanan değer ile belirlenmektedir. Diğer taraftan bazı Button kontrollerinin Margin özellikleri ile oynanarak hücre içerisindeki kenar boşluklarına ait miktarlarda belirlenmektedir. Söz gelimi ilk düğme hücrenin tüm kenarlarına 5 piksel uzaklıkta olacaktır. Diğer taraftan 2 yazılı Button bileşeni sol tarafa 10 piksel, hücrenin üst tarafına ise 15 piksel uzaklıkta olacak şekilde yer kaplayacaktır. Sonuç olarak tasarım zamanındaki ekran çıktısı aşağıdaki gibi olacaktır.

![mk257_9.gif](/assets/images/2008/mk257_9.gif)

Grid;

Grid bileşeninde satırlar ve sütunlar geliştirici tarafından daha detaylı bir şekilde ayarlanır. Bu da söz konusu ızgara üzerinde çok daha fazla geliştirici kontrolü olacağı anlamına gelmektedir. Grid bileşeni içerisinde yer alacak elementlerin hangi hücrelere geleceğini belirlemek için yine Attached Property tekniğinden yararlanılmaktadır. Buna göre Row ve Column özelliklerine atanan değerler ile yerleşim hücresi belirlenir. Bu özelliklerin varsayılan değeri 0' dır. Buna göre kontrol ilk hücreye atanır. Grid kontrolünün hücreleri üzerine Border kullanımıda gerçekleştirilebilir. Grid kontrolüne ait örnek bir kullanım aşağıdaki XAML içeriğinde olduğu gibi göz önüne alınabilir.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <Grid ShowGridLines="True"> 
        <Grid.Background>
            <LinearGradientBrush>
                <LinearGradientBrush.GradientStops>
                    <GradientStop Color="AliceBlue" Offset="0.50"/> 
                    <GradientStop Color="Gold" Offset="1"/>
                </LinearGradientBrush.GradientStops>
            </LinearGradientBrush>
        </Grid.Background>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="25"/>
            <ColumnDefinition Width="50"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        <Border Background="BlueViolet" BorderThickness="2" Grid.Row="2" BorderBrush="Red" Margin="4" Grid.ColumnSpan="2"/>
        <TextBlock Text="No" VerticalAlignment="Bottom" HorizontalAlignment="Right"/>
        <TextBlock Text="Ad" VerticalAlignment="Bottom" HorizontalAlignment="Right" Grid.Row="0" Grid.Column="1"/>
        <TextBlock Text="Yorum" VerticalAlignment="Bottom" HorizontalAlignment="Right" Grid.Row="0" Grid.Column="2"/>
        <Rectangle Fill="RosyBrown" Grid.Row="1" Grid.ColumnSpan="3" Height="3"/>
        <TextBlock Text="10" VerticalAlignment="Center" HorizontalAlignment="Center" FontWeight="Bold" Foreground="White" Grid.Row="2"/>
        <TextBlock Text="Burak Selim Şenyurt" TextWrapping="Wrap" VerticalAlignment="Center" HorizontalAlignment="Center" FontWeight="Bold" Foreground="White" Grid.Row="2" Grid.Column="1"/>
    </Grid>
</Window>
```

İlk olarak Grid kontrolünün arka plan dolgusu (Background) LinearGradientBrush kullanılarak AliceBlue renginden Gold rengine değişecek şekilde belirlenmiştir. Grid içerisindeki satırları belirlemek için RowDefinitions (RowDefinitionCollection tipinden), sütunları belirlemek içinse ColumnDefinitions (ColumnDefinitionCollection tipinden) koleksiyonları kullanılır. Satırlar RowDefinition elementi ile tanımlanırken, sütunlar ColumnDefinition ile tanımlanmaktadır. Satırlar için yükselik değeri Height özelliği ile, sütunlar için genişlik değeri Width özelliği ile belirlenir.

Dikkat çekici noktalardan biriside Auto ve kullanımıdır. Auto ifadesine göre yükseklik veya genişlik değeri içerideki elementin boyutuna göre otomatik olarak ayarlanır. Diğer taraftan, "kalan tüm mesafeyi kullan" anlamında düşünülebilir. Örneğin pencerenin genişliği 250 pikseldir. İlk sütun 25, ikinci sütun ise 50 olarak belirlenmektedir. Geriye kalan 175 piksel ise üçüncü sütuna işareti ile bırakılmaktadır. Grid içerisinde kullanılan Rectangle elementinden Grid.ColumnSpan özelliğine 3 değeri verilmiştir. Buna göre Rectangle elementinin bulunduğu 1nci satırdaki 3 hücre birleştirilmektedir. Yine VerticalAlignment ve HorizontalAlignment özelliklerine atanan değerler kullanılarak kontrolün hücre içerisindeki konumu belirlenebilir. Grid kontrolü ile ilişkili ilginç elementlerden biriside Border bileşenidir. Örnekte kullanılan Border elementi Grid bileşeninin 2nci satırıda yer alan ilk iki hücresine (Grid.ColumnSpan=2 nedeniyle) uygulanmaktadır. Sonuç itibariyle Grid bileşeninin çalışma zamanında verdiği ekran çıktısı aşağıdaki gibi olacaktır.

![mk257_10.gif](/assets/images/2008/mk257_10.gif)

Canvas;

Son olarak Canvas bileşenine bir göz atalım. Bu kontrolde bileşenlerin konumlarını belirlemek için Top, Lef, Right veya Bottom gibi özelliklerden yararlanılmaktadır. Varsayılan olarak bu değerler belirtilmediği takdirde bileşen 0,0 noktasına konumlandırılmaktadır. Bir başka deyişle Canvas bileşeninin sol süt köşesine yanaştırılmaktadır. Canvas bileşeni için örnek olarak aşağıdaki XAML içeriği göz önüne alınabilir.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="150" Width="250">
    <Canvas Background="LightGray">
        <Button x:Name="Button1" Content="Button 1"/>
        <Button x:Name="Button2" Content="Button 2" Canvas.Left="100" Canvas.Top="50" />
        <Button x:Name="Button3" Content="Button 3" Canvas.Right="100" Canvas.Bottom="50" />
    </Canvas>
</Window>
```

Dikkat edileceği üzere Button1 için herhangibir konum değeri belirtilmemiştir. Button2 için ise Left ve Top özellikleri kullanılarak konumlandırma yapılmaktadır. Son Button için ise Right ve Bottom özellikleri kullanılmaktadır. Yine dikkat edilmesi gereken noktalardan birisi Attached Property kullanılmış olmasıdır. (Böylece ilgili element içerisinde, dahil olduğu elemente ait özelliklere nokta notasyonu ile erişilebilmektedir.) Tasarım zamanındaki ekran görüntüsü aşağıdaki gibi olacaktır.

![mk257_11.gif](/assets/images/2008/mk257_11.gif)

Dikkat edilmesi gereken noktalardan biriside Button3' ün element sırasına göre Button2' nin üstünde çıkmış olmasıdır. Bu son derece doğaldır. Ama istenirse Button2' nin önde durmasıda sağlanabilir. Bunun için Panel sınıfının ZIndex özelliği kullanılmaktadır. Söz gelimi yukarıdaki XAML içeriği aşağıdaki gibi değiştirilebilir.

![mk257_12.gif](/assets/images/2008/mk257_12.gif)

Dikkat edileceği üzere Button2 için Panel.ZIndex değeri 1 olarak set edilmiştir. Buna göre Button2 element sırasına bakılmaksızın en öne gelmiştir.

Buraya kadar geliştirilen basit örnekler ile WPF uygulamalarında kullanılabilecek temek Layout bileşenleri incelenmeye çalışılmıştır. Bu Layout bileşenlerinin ihtiyaçları karşılamaması halinde ise istenirse Panel abstract sınıfından türetme yoluna gidilerek farklı bir bileşenin üretilmesi sağlanabilir. Window bileşenleri kendi içlerinde sadece tek bir Panel taşıyabilirler. Bir başka deyişle iki Layout kontrolünü Window elementi altında aynı seviyede kullanamayız. Ancak bu kısıtlama, Layout içerisinde Layout kullanılmasını engellemez. Nitekim çoğu durumda bir Layout bileşeni içerisinde farklı bir Layout bileşeni kullanılması gerekebilir. Söz gelimi aşağıdaki örnek XAML içeriğinde bu durum gösterilmeye çalışılmaktadır.

```xml
<Window x:Class="Layouts.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Layout kullanımı" Height="250" Width="250" Loaded="Window_Loaded">
    <Grid Background="Gold" ShowGridLines="True">
        <Grid.RowDefinitions>
            <RowDefinition Height="100"/>
            <RowDefinition Height="*"/> <!-- İkinci satır kalan tüm kısmı kaplayacaktır-->
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="75"/>
            <ColumnDefinition Width="*"/> <!-- İkinci sütun kalan tüm kısmı kaplayacaktır-->
        </Grid.ColumnDefinitions>
        <Button Content="Gönder" Height="40" Margin="5,0,10,0"/> <!-- Sola 5, sağa 10 piksel uzaklıkta olacaktır-->
        <StackPanel Grid.Row="1" Grid.ColumnSpan="2" Background="AliceBlue" Margin="5"> <!-- İkinci satırdaki iki sütun birleştirilir. StackPanel bu bölüme eklenir.-->
            <TextBlock Text="Mesajınız Yazınız"/>
            <TextBox Text="" Width="100" HorizontalAlignment="Right" TextWrapping="Wrap" Height="75" ScrollViewer.VerticalScrollBarVisibility="Visible" Margin="0,0,10,0"/> <!-- Sağdan 10 piksel uzaklıkta, 100 piksel genişliğinde yatay olarak sağa yaslanmış Wrap özelliği açık, 75 piksel yüksekliğinde ve dikey kaydırma çubuğu görünür olan TextBox-->
        </StackPanel>
        <UniformGrid Grid.Column="1" Margin="2"> <!-- Bu UniformGrid 1nci sütun içerisinde yer almaktadır.-->
            <Button>1</Button>
            <Button>2</Button>
            <Button>3</Button>
            <Button>4</Button>
            <Button>5</Button>
            <Button>6</Button>
            <Button>7</Button>
            <Button>8</Button>
            <Button>9</Button>
            <Button>0</Button>
        </UniformGrid>
    </Grid>
</Window>
```

Örnek XAML içeriğine ait tasarım zamanı ekran çıktısı aşağıdaki gibi olacaktır.

![mk257_13.gif](/assets/images/2008/mk257_13.gif)

Elbetteki Layout bileşenlerinin dinamik olarak kod içerisinde ele alınmasıda mümkündür. Senaryonun karmaşıklığına göre Visual Studio 2008 IDE'si kullanılarak Layout'ların ve içeriklerinin tasarlanmasında çok daha iyi sonuçlar alınabilir. Hatta Expression ailesindeki ürünlerden yararlanılarak bu görsel bileşenlerin profesyonel görünümlere sahip olarak ürünsel nitelikte olmasıda daha rahat bir şekilde sağlanabilir. Ancak bunların öncesinde XAML tarafında ilgili bileşenlerin bu makalede olduğu gibi nasıl kullanılabileceğinin bilinmesinde yarar vardır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/Layouts.rar)
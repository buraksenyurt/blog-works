---
layout: post
title: "Asp.Net 2.0 GridView Kontrolünde Update,Delete İşlemleri"
date: 2004-09-07 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - dotnet
  - aspnet
  - http
---
Bu makalemizde, Asp.Net 2.0 ile gelen yeni kontrollerden birisi olan GridView kontrolü üzerinde, veri güncelleme ve veri silme gibi işlemlerin nasıl yapılacağını incelemeye çalışacağız. Asp.Net 2.0 ve dolayısıyla Framework 2.0, özellikle veri bağlı kontrollerde, yazılım geliştiricilerin sıklıkla yaptıkları rutin işlemlerin dahada kolaylaştırılmasına izin veren mimari yaklaşımları benimsemektedir.

Bu makalemize konu olan GridView kontrolü internet sayfalarında verilerin eski DataGrid kontrolünde olduğu gibi ızgara formatında görünmesini sağlar. Bununla birlikte GridView kontrolü üzerinde sayfalama ve sıralama gibi işlemleri yapmak eskiye nazaran çok daha kolaydır. İşte GridView kontrolünün yazılım geliştirici dostu olan yapısının bir diğer kolaylığıda, satırlar üzerinde güncelleme ve silme işlmelerine getirdiği yeniliklerde gizlidir.

Bu makalemizde geliştireceğimiz örneğimizde, bir Access veri kaynağına bağlanacağımız bir web sayfamız olacak. Bu sayfada kullancağımız GridView kontrolünü veri güncelleme ve silme işlemleri için kullanacağız. Bu örneği geliştirebilmemiz için, GridView kontrolünü Access veritabanımıza bağlayacak bir DataSource kontrolüne ihtiyacımız var. Access veritabanlarına bağlanmak ve ilgili veritabanı üzerindeki tablolarda işlem yapabilmek için,.net Framework 2.0 ile gelen yeni nesnelerden birisi olan AccessDataSource kontrolünü kullanacağız. Veritabanımız Access ile geliştirilmiş olup MailListesi isimli aşağıdaki Field yapısına sahip bir tabloyu barındırmaktadır.

![mk89_1.gif](/assets/images/2004/mk89_1.gif)

Şekil 1. Access tablomuzun field yapısı.

Oluşturduğumuz bu tabloyu, web sitemiz için standart olarak açılan Data klasörü altına kopyalayım.

![mk89_12.gif](/assets/images/2004/mk89_12.gif)

Şekil 2. Veritabanımızı Data klasörü altına koyalım.

Sıra geldi default.aspx sayfamızı tasarlamaya. Öncelikli olarak, AccessDataSource kontrolümüzü aşağıdaki aspx kodları ile sayfaya ekleyelim.

```text
<asp:AccessDataSource ID="AccessDataSource1" Runat="server" DataFile="~/Data/veriler.mdb"
SelectCommand="SELECT * FROM [MailListesi]" UpdateCommand="UPDATE [MailListesi] SET Ad=@AD,Soyad=@SOYAD,Mail=@MAIL WHERE ID=@ID">
    <UpdateParameters>
        <asp:Parameter Name="AD"></asp:Parameter>
        <asp:Parameter Name="SOYAD"></asp:Parameter>
        <asp:Parameter Name="MAIL"></asp:Parameter>
        <asp:Parameter Name="ID"></asp:Parameter>
    </UpdateParameters>
</asp:AccessDataSource>
```

Burada dikkat ederseniz, SelectCommand özelliği dışında, dikkati çeken bir diğer özellik UpdateCommand'dır. UpdateCommand özelliği, satırlardaki değişikliklerin veri kaynağına yansıtılmasında kullanılacak Sql sorgusunu içermektedir. Bu sql sorgusu dikkat edecek olursanız, çeşitli parametrelere sahiptir. Dikkate değer diğer bir noktada, parametre bildirimlerinin, Framework 1.0/1.1' de olan, OleDbCommand kontrollerindeki gibi,? (soru işareti) olmayışıdır. Bu noktada, Sql sunucuları üzerinde çalıştırılan parametreli sorgulardaki yapının (@Parametre_adı yapısı) aynısının Access kaynakları içinde kullanılabildiğini ve böylece ortak bir standardın oluşturulmuş olduğunuda belirtelim.

UpdateCommand özelliğindeki Sql sorgusunda yer alan parametreler, AccessDataSource kontrolünün UpdateParameters koleksiyonunda birer Parameter nesnesi olarak tanımlanırlar. Basitçe burada parametrelerimizin isimlerini Name özellikleri ile belirledik. DataSource bileşenleri için, SelectCommand ve UpdateCommand komutları dışında kullanılabilecek diğer komutlarda, DeleteCommand ve InsertCommand'dir. Bunların herbirinin parametre koleksiyonları farklıdır. Yani DeleteCommand için parametre koleksiyonu DeleteParameters iken, InsertCommand için InsertParameters'dır.

![mk89_2.gif](/assets/images/2004/mk89_2.gif)

Şekil 3. DataSource'lar için geçerli diğer parametre koleksiyonları.

AccessDataSource bileşenimiz artık veri çekmek ve güncellemek için hazırdır. Sırada, GridView kontrolümüzü, bu veri kaynağına göre düzenlemek var. Bunun için, GridView kontrolümüzü aşağıdaki aspx kodları ile oluşturmamız yeterli olacaktır.

```text
<asp:GridView ID="GridView1" Runat="server" DataSourceID="AccessDataSource1" DataKeyNames="ID" AutoGenerateColumns="False">
    <Columns>
        <asp:CommandField ShowEditButton="true"></asp:CommandField>
        <asp:BoundField ReadOnly="True" HeaderText="ID" InsertVisible="False" DataField="ID"
SortExpression="ID"></asp:BoundField>
        <asp:BoundField HeaderText="Ad" DataField="Ad" SortExpression="Ad"></asp:BoundField>
        <asp:BoundField HeaderText="Soyad" DataField="Soyad" SortExpression="Soyad"></asp:BoundField>
        <asp:BoundField HeaderText="Mail" DataField="Mail" SortExpression="Mail"></asp:BoundField>
    </Columns>
</asp:GridView>
```

Şu aşamada bizim için en önemli özellik, DataKeyNames'dir. Bu özellik, GridView kontrolünde üzerindeki satırlarda yapılan veri değişiklikleri sonucu, güncelleme işlemlerinin hangi primary key alanı üzerinden yapılacağını belirtmektedir. Nitekim, Sql sorgumuza dikkat edecek olursanız, güncelleme işleminin hangi alan için yapılacağını belirleyebilmek amacıyla, primary key olan ID alanı kullanılmıştır.

```text
UPDATE [MailListesi] SET Ad=@AD,Soyad=@SOYAD,Mail=@MAIL WHERE ID=@ID
```

Bu sebeple, GridView kontrolü, Update butonuna basıldığında, DataKeyNames özelliğindeki field alanının değerini, seçili olan satır üzerinden AccessDataSource kontrolündeki UpdateCommand sorgusuna gönderecektir. Diğer önemli üye ise, CommandField kontrolüdür. Bu kontrol, satır üzerinde hangi işlemin yapılacağı ile ilgili link (button) kontrollerinin gösterilmesi için kullanılmaktadır. Burada ShowEditButton özelliğine true değerini vererek, ilgili satır için Edit, Update ve Cancel linklerinin gösterilmesini sağlamış olduk. Artık web formumuza son halini verebiliriz.

![mk89_3.gif](/assets/images/2004/mk89_3.gif)

Şekil 4. default.aspx sayfamızın görünümü.

Şimdi sayfamızı tarayıcı penceresinde açar ve herhangibir satırda edit linkine tıklayacak olursak, aşağıdaki görünümü elde ederiz.

![mk89_4.gif](/assets/images/2004/mk89_4.gif)

Şekil 5. Edit linkine tıklandığında.

Görüldüğü gibi, ilgili satıra ait alanlarda TextBox kutucukları veri girişi için hazır beklemektedir. Bu noktada Update linkine tıklarsak, seçili satırın ID değeri ile birlikte diğer alanların güncel içerikleri, AccessDataSource kontrolünün UpdateCommand özelliğinde kullanılan Sql sorgusuna parametre olarak gönderilirler. Ardından güncelleme işlemi yapılır ve GridView kontrolü güncel içeriği ile görüntülenir. Örneğin, "Burak S." değerini "Burak Selim" yapıp Update linkine bastığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk89_5.gif](/assets/images/2004/mk89_5.gif)

Şekil 5. Güncelleme işlemi sonrası.

Tahmin edin, Delete işlemini nasıl gerçekleştireceğiz. Uygulayacağımız mantık aslında, Update işlemi için uygulanan ile aynı olacaktır. İlk olarak AccessDataSource kontrolümüzde, DeleteCommand özelliğini tanımlamalı ve geçerli bir Delete sql sorgusu girmeliyiz. Ardından bu Sql sorgusunda kullanacağımız ID alanı için bir parametre ekelemeliyiz. Yani AccessDataSource kontrolümüzü aşağıdaki hale getirmeliyiz.

```text
<asp:AccessDataSource ID="AccessDataSource1" Runat="server" DataFile="~/Data/veriler.mdb"
SelectCommand="SELECT * FROM [MailListesi]" UpdateCommand="UPDATE [MailListesi] SET Ad=@AD,Soyad=@SOYAD,Mail=@MAIL WHERE ID=@ID" DeleteCommand="DELETE FROM [MailListesi] WHERE ID=@ID">
    <UpdateParameters>
        <asp:Parameter Name="AD"></asp:Parameter>
        <asp:Parameter Name="SOYAD"></asp:Parameter>
        <asp:Parameter Name="MAIL"></asp:Parameter>
        <asp:Parameter Name="ID"></asp:Parameter>
    </UpdateParameters>
    <DeleteParameters>
        <asp:Parameter Name="ID"></asp:Parameter>
</DeleteParameters>
```

Tabi GridView kontrolümüzde de Delete işlemi için gerekli linki (button'u) göstermeliyiz. Bu amaçlada, GridView kontrolünün CommandField bileşeninin ShowDeleteButton özelliğinin değerini true yapmalıyız.

```text
<asp:CommandField ShowDeleteButton="true" ShowEditButton="true"></asp:CommandField>
```

Bu değişikliklerden sonra sayfamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk89_6.gif](/assets/images/2004/mk89_6.gif)

Şekil 7. Delete linkinin eklenmesi.

Eğer Delete linki ile bir satırı silecek olursak, ilgili satırın ID değeri, AccessDataSource kontrolündeki, DeleteCommand özelliğine parametre olarak gidecek ve Sql sorgusu çalıştırılarak ilgili satır silinecektir. Örnek olarak, 1 ID değerine sahip satırı sildiğimizde, aşağıdaki ekran görüntüsünü elde ederiz.

![mk89_7.gif](/assets/images/2004/mk89_7.gif)

Şekil 8. Satır silindikten sonra.

Bununla birlikte unutulmaması gereken önemli bir nokta vardır. Buradaki tüm işlemler veri tablosuna doğrudan yansıtılır. Dolayısıyla, satırı sildiğimizde (güncellediğimizde), AccessDataSource bunu kesinlikle, asıl tabloyada yansıtacaktır. Nitekim son işlemden sonra, tablomuza bakacak olursak ID değeri 1 olan satırın artık olmadığını görebiliriz.

![mk89_8.gif](/assets/images/2004/mk89_8.gif)

Şekil 9. Satır kesin olarak silinir.

Geliştirdiğimiz örneği biraz makyajlamaya ne dersiniz? Örneğin, Update, Edit, Cancel ve Delete kelimeleri yerine Türkçe bir şeyler çıksa ve bunlar Link yerine button olsa fena olmazdı değil mi? Bunun için tek yapmamız gereken aşağıdaki aspx kodları ile, GridView kontrolümüzü güncellemektir. Dikkat edecek olursanız, her bir kontrolün sonu Text kelimesi ile biten bir özelliği vardır. Örneğin Delete butonu için DeleteText. Dolayısıyla bunları değişitirerek ekranda görünmesini istediğimiz metini belirtebiliriz. Ayrıca, linklerimizi button haline getirmek için, ButtonType özelliğine Button değerini atadık.

```text
<asp:CommandField ButtonType="Button" CancelText="İptal" EditText="Düzenle" DeleteText="Sil" UpdateText="Güncelle" ShowDeleteButton="true" ShowEditButton="true"></asp:CommandField>
```

![mk89_9.gif](/assets/images/2004/mk89_9.gif)

![mk89_10.gif](/assets/images/2004/mk89_10.gif)

Şekil 10. Türkçe buttonlar.

Sayfamızın son olarak makyajlanmış hali ve ilgili aspx kodları aşağıdaki gibidir.

![mk89_11.gif](/assets/images/2004/mk89_11.gif)

Şekil 11. Sayfamızın biraz daha makyajlı hali.

```text
<%@ Page Language="C#" CompileWith="Default.aspx.cs" ClassName="Default_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:AccessDataSource ID="AccessDataSource1" Runat="server" DataFile="~/Data/veriler.mdb"
SelectCommand="SELECT * FROM [MailListesi]" UpdateCommand="UPDATE [MailListesi] SET Ad=@AD,Soyad=@SOYAD,Mail=@MAIL WHERE ID=@ID" DeleteCommand="DELETE FROM [MailListesi] WHERE ID=@ID">
            <UpdateParameters>
                <asp:Parameter Name="AD"></asp:Parameter>
                <asp:Parameter Name="SOYAD"></asp:Parameter>
                <asp:Parameter Name="MAIL"></asp:Parameter>
                <asp:Parameter Name="ID"></asp:Parameter>
            </UpdateParameters>
            <DeleteParameters>
                <asp:Parameter Name="ID"></asp:Parameter>
            </DeleteParameters>
        </asp:AccessDataSource><br/>
        <b><span style="font-size: 24pt; color: #cc0000; font-family: Agency FB">Mail Listesi</span></b>
        <asp:GridView ID="GridView1" Runat="server" DataSourceID="AccessDataSource1" DataKeyNames="ID" AutoGenerateColumns="False" BorderWidth="1px" BackColor="White" GridLines="Vertical" CellPadding="4" BorderStyle="None" BorderColor="#DEDFDE" ForeColor="Black" AllowSorting="True" AllowPaging="True">
            <FooterStyle BackColor="#CCCC99"></FooterStyle>
            <PagerStyle ForeColor="Black" HorizontalAlign="Right" BackColor="#F7F7DE"></PagerStyle>
            <HeaderStyle ForeColor="White" Font-Bold="True" BackColor="#6B696B"></HeaderStyle>
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <Columns>
                <asp:CommandField ButtonType="Button" CancelText="İptal" EditText="Düzenle" DeleteText="Sil" UpdateText="Güncelle"     ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
                <asp:BoundField ReadOnly="True" HeaderText="ID" InsertVisible="False" DataField="ID"
SortExpression="ID"></asp:BoundField>
                <asp:BoundField HeaderText="Ad" DataField="Ad" SortExpression="Ad"></asp:BoundField>
                <asp:BoundField HeaderText="Soyad" DataField="Soyad" SortExpression="Soyad"></asp:BoundField>
                <asp:BoundField HeaderText="Mail" DataField="Mail" SortExpression="Mail"></asp:BoundField>
            </Columns>
            <SelectedRowStyle ForeColor="White" Font-Bold="True" BackColor="#CE5D5A"></SelectedRowStyle>
            <RowStyle BackColor="#F7F7DE"></RowStyle>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
```

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
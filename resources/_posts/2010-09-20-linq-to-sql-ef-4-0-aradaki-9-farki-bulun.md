---
layout: post
title: "LINQ to SQL – EF 4.0 (Aradaki 9 Farkı Bulun)"
date: 2010-09-20 15:07:00 +0300
description: "Merhaba arkadaşlar,"
categories:
  - entity-framework
  - linq-to-sql
tags:
  - entity-framework
  - linq-to-sql
  - dotnet
  - linq
  - sql-server
  - xml
---
# LINQ to SQL – EF 4.0 (Aradaki 9 Farkı Bulun)
Merhaba arkadaşlar,

Evet çok doğru. Hiç bu kadar kısa ve öz yazmamıştım daha önceden. Ama zaman zaman bu kadar kısa yazıp çok fazla şey ifade edilebileceğine de inanmaktayım

![Wink](/assets/images/2010/smiley-wink.gif)

Hani ilk bakışta herşeyin şak diye kafanızda yer ettiği tablolar olur ya…Bu blog girdisinde de benzer bir resmi göreceğinizi düşünüyorum. Aslında olay bundan çok çok uzun zaman önce gelen bir soru üzerine meydana geldi.

“Entity Framework ile LINQ to SQL arasındaki temel ve belirgin farklılıklar nelerdir? Hangi durumlarda hangisi tercih edilmelidir?”

Soruyu araştırmak ve cevap vermek için aradan baya bir zaman geçti ancak yaptığım incelemeler sonucunda aşağıdaki temel ayrımları tespit ettiğimi ifade edebilirim. Hatta ortak oldukları noktaları da bu tabloda görebilirsiniz.

Kavram
EF 4.0
LINQ to SQL

Sql Server Harici Veritabanı Desteği
Var
Yok gibi

Doğrudan veritabanı bağlantısı
Yok
Var

Çoklu tablodan kalıtım (Multiple Table Inheritance)
Var
Yok

Birden fazla tablodan tek bir Entity üretmek
Var
Yok

Conceptual Schema Definition Language (CSDL)
Var
Yok

Storage Schema Definition Language (SSDL)
Var
Yok

Mapping Schema Language (MSL)
Var
Yok

Lazy Loading
Var
Var

Stored Procedures
Var
Var

Tabi bazı noktalarda çok keskin ayrımlar olduğunu ifade edebiliriz. Söz gelimi LINQ to SQL takımı doğrudan.Net’ in SQL yönetim bileşenlerini kullanmayı tercih etmektedir. Sanıyorum ki C# takımının üyelerinin LINQ to SQL’ i geliştirmiş olmasının bunda büyük rolü vardır

![Wink](/assets/images/2010/smiley-wink.gif)

Öyleki EF takımı doğrudan veritabanı erişimi veya SQL nesnelerini (SqlConnection, SqlCommand vb…) kullanmak yerine Conceptual bir modeli baz alarak depolama ve kütüphane nesnelerinin eşleştirilmesinde XML bazlı daha esnek bir yapıyı tercih etmiştir. Dilerseniz LINQ to SQL’ in arka tarafına kısaca bir bakalım ve SQL nesne kullanımını (ya da bağımlılığını) görmeye çalışalım.

Örnek olarak Northwind veritabanını kullandığımız bir LINQ to SQL sınıf yapsında üretilen NorthwindDataContext tipinin içeriğinde aşağıdaki yapıcı metodlar (Constructors) hemen göze çarpacaktır.

[![blg228_DataContext](/assets/images/2010/blg228_DataContext_thumb.gif)](/assets/images/2010/blg228_DataContext.gif)

Dikkat edileceği üzere IDbConnection interface tipini kullanan iki yapıcı metod versiyonu söz konusudur. Eğer IDBConnection’ dan türeyen.Net tiplerine bakarsak aşağıdaki sonuçlar ile karşılaşırız.

[![blg228_IDbConnection](/assets/images/2010/blg228_IDbConnection_thumb.gif)](/assets/images/2010/blg228_IDbConnection.gif)

Dikkat edeceğiniz üzere OdbcConnection, OleDbConnection ve SqlConnection tipleri söz konusudur. Buradan DataContext türevli olan NorthwindDataContext tipinin üretimi sırasında mutlaka SQL tarafına bir bağımlılığın olduğunu en azından.Net içerisinde var olan provider yapısının ele alındığını görebiliriz.

Bu ve diğer farklılıkları aslında ilerleyen yazılarımızda incelemeye çalışıyor olacağım. Siz şimdilik yukarıdaki klavuzu gözünüze kestirin. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
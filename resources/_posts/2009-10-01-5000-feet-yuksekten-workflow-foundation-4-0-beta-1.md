---
layout: post
title: "5000 Feet Yüksekten Workflow Foundation 4.0[Beta 1]"
date: 2009-10-01 13:54:00 +0300
description: "Paraşütle atlamak gerçekten zevkli olsa gerek. Yerden binlerce feet(1 feet=30,48 cm) yüksekten atlayıp özgür bir şekilde kendinizi yer çekimi gücüne bırakıp, saniyeler boyunca serbest düşüşü yaşamak...Size yandaki resimde atlayan kişinin ben olduğumu söylemek isterdim ama ne yazık ki değilim. Olmayı istermiydim bilemiyorum. Oldukça yüksek görünüyor. Sealed Bir paraşütçü için en güzel duygulardan birisi sanıyorum ki atladığı noktadan itibaren altındaki Dünyayı görebildiği kadar yüksekten izleyebilmenin verdiği mutluluktur."
categories:
  - wf-4-0-beta-1
tags:
  - wf-4-0-beta-1
  - dotnet
  - wcf
  - workflow-foundation
  - wpf
  - xaml
  - http
  - testing
  - visual-studio
---
# 5000 Feet Yüksekten Workflow Foundation 4.0[Beta 1]
![blg86_Giris.jpg](/assets/images/2009/blg86_Giris.jpg)

Merhaba Arkadaşlar,

Paraşütle atlamak gerçekten zevkli olsa gerek. Yerden binlerce feet (1 feet=30,48 cm) yüksekten atlayıp özgür bir şekilde kendinizi yer çekimi gücüne bırakıp, saniyeler boyunca serbest düşüşü yaşamak...Size yandaki resimde atlayan kişinin ben olduğumu söylemek isterdim ama ne yazık ki değilim. Olmayı istermiydim bilemiyorum. Oldukça yüksek görünüyor.

![Sealed](/assets/images/2009/smiley-sealed.gif)

Bir paraşütçü için en güzel duygulardan birisi sanıyorum ki atladığı noktadan itibaren altındaki Dünyayı görebildiği kadar yüksekten izleyebilmenin verdiği mutluluktur.

Tabiki atlanılan noktadan aşağıya doğru düştükçe ve paraşütü çekme noktasına yaklaştıkça alttaki Dünyanın resimlerinin daha da büyüdüğü çok açık bir gerçektir. Büyük bir dikdörten alan...Sonrasında içerisinde başka geometrik şekiller...Sonrasında bu geometrik şekiller içerisinde daha da netleşen çayırlar, dağlar, kayalar, yollar, binalar...Sonrasında bir anlık yavaşlama ve sakin bir şekilde (bazende hızlı bir şekilde) yere ayak basmak. Kısaca yaşanan bu duruma "yüksekten görülebileceği kadar büyük bir alanı görüp, yaklaştıkça daha fazla detay fark edebilmek durumu"

![Cool](/assets/images/2009/smiley-cool.gif)

adını verebiliriz. Şimdi bu noktaya nereden vardım diyebilirsiniz. Hemen açıklayayım.

Yazılım ile ilişkili pek çok kaynakta şu tip başlıklar görmüşsünüzdür. 50 bin feet yukarıdan X mimarisi...İşte bizde bu felsefeyi bu günkü blog yazımızda kullanıyor olacağız. Ama biraz daha alçak mesafeden

![Wink](/assets/images/2009/smiley-wink.gif)

5000 feet yukarıdan fotoğraflandığında, WF 4.0 modelinin ne gibi özellikleri dikkat çekiyor? İşte görülen tespitler...

- WF 3.X'teki kısıtlı olan XAML bazlı Workflow modeli yerini tam desteklenen (Full XAML Based) Workflow modeline bırakıyor. Böylece bir Workflow'un basit bir text editorü yardımıyla tamamen dekleratif (declerative) olarak geliştirilmesi ve çalışma zamanına devredilmesinin mümkün olabildiğini (Bu özelliğin en önemli açılımlarından biriside, çeşitli 3ncü parti araçların XAML içeriklerini ele alarak akışları koda girmeden değiştirebilecek olmaları. Oslo, Dublin ve Quadrant üçlemesinin önemle üzerinde durduğu noktalardan biriside zaten bu dekleratif açılım.)
- WF 3.X'te daha zor olan özel aktivite (Custom Activity) geliştirme yeteneğinin WF 4.0 için daha da basitleştirildiğini ve bu amaçla temel workflow hiyerarşisinde de değişikliklere gidildiğini ve ata tip olarak WorkflowElement ve kendisinden türeyen Activity, CodeActivity, NativeActivity gibi alt tiplerin geliştirildiğini,
- WF 3.X'te ilkel ve daha az genişletilebilir olan kurallar motorunun (Rules Engine) dahada zenginleştirilmiş olduğunu,
- Yeni Workflow bileşenlerinin System.Activities. assembly'ları altında olduğunu ama.Net Framework 4.0 içerisinde yer alan ve geriye uyumluluk (Backwards Compatibility) amacıyla kullanılan Workflow bileşenlerininse System.Workflows. assembly'ları içerisinde yer aldığını, bu anlamda WF 4.0' da geriye uyumluluğa da büyük önem verildiğini,
- Tamamen WPF (Windows Presentation Foundation) temelli bir tasarım ortamının söz konusu olduğunu ve bu sayede geliştirici deneyiminin dahada zenginleştiğini,
- Bir Workflow içerisine veya dışarısına yapılan veri akışlarının (Data Flow) çok daha kolay ele alınması için Designer desteği ile birlikte Arguments kavramının geldiğini,
- Bir aktivitenin kendi içerisinde veriyi saklaması ve farklı seviyedeki alanlarda (Scope) kullanabilmesinde rol oynayan Variables kavramını ayrıca Arguments kavramında olduğu gibi, Variables içinde Designer desteğinin olduğunu,
- Bir veya daha fazla input argümanı alıp bunlar üzerinde çeşitli operasyonlar gerçekleştiren ve geriye değer döndürebilen ifadeler (Expressions) yazılabildiğini, üstelik bunların XAML bazlı olabildiğini,
- FlowChart, ForEach, Parallel, ParallelForEach (Parallel versiyonların Ekimdeki [PDC'](http://microsoftpdc.com/)de yayınlanacak sürümde olması bekleniyor) ve daha pek çok aktivite tipi ile zengineştirilmiş olan temel aktivite kütüphanesi (Base Activity Library) ni,
- Sequential ve State Machine arasında duran ama geliştirici ve iş analistlerinin, bilinen iş akışı tasarım modeline çok yakın olması nedeniyle kolayca kullanabildiği yeni Flow Chart modelini,
- Workflow ve Activity'ler için Unit Test'lerin kolayca geliştirilebiliyor olduğunu,
- Workflow'ların, dışarıdaki Activity'ler ile haberleşmenin dahada kolaylaştırılmış olduğunu,
- 4.0 versiyonunda evlenebilmerleri için WCF ve WF tarafında;
  - Workflow tarafında yenilenen çalışma zamanı motoru bulunduğunu,
  - Workflow Service'lerin host edilebiliyor olduğunu,
  - Workflow'lar içerisinde XAML bazlı olarak WCF servis materyallerinin tanımlanabiliyor olduğunu,(Service Contract, Data Contract, EndPoint vb...)
  - Visual Studio'da Workflow Service'ler için Add Service Reference desteğinin getirildiğini,
  - Yeni mesajlaşma aktivitileri (SendAndReceiveReply, ReceiveAndSendReply gibi) ve bunların mesaj korelasyon (Message Corellation) desteğine de sahip olduğunu,
- WF 3.X ile yazılmış olan Workflow'ların 4.0 çalışma zamanı tarafından da yürütülebildiğini,
- WF 3.X tarafından yazılmış olan aktivitelerin sarmalanarak (Wrap) 4.0 içerisinde de kullanılabildiğini,(Interop activity)
- Ayrıca WF 3.0' dan geçiş yapacaklar için bir [klavuzun](http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=bd94c260-b5e0-4d12-93ec-53567505e685)bulunduğunu,
- Daha detaylı bilgiler içinse[şu adrese](http://blogs.msdn.com/endpoint/archive/2009/05/01/the-road-to-4-wf-changes-between-beta-1-and-ctp.aspx)başvruabileceğimizi,

görüyoruz. Vooovvvvv!!!! Artık paraşütümüzü açalım mı ne dersiniz?

![Cool](/assets/images/2009/smiley-cool.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
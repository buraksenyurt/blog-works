---
layout: post
title: "Task Parallel Library(TPL) - İptal İşlemi [Beta 2]"
date: 2009-11-12 02:00:00 +0300
description: "Uzun süredir .Net Framework 4.0' ın bir parçası olarak gelen paralel programlama alt yapısı ile uğraşmıyordum. En son Beta 1 sürümündeyken Task Parallel Library ve PLINQ ile ilişkili konulara bakma fırsatım olmuştu. Zaman ilerledi ve .Net Framework 4.0 Beta 2 sürümü yayınlandı."
categories:
  - tpl
tags:
  - tpl
  - csharp
  - dotnet
  - wcf
  - workflow-foundation
  - parallel-programming
  - task-parallel-library
---
# Task Parallel Library(TPL) - İptal İşlemi [Beta 2]
Merhaba Arkadaşlar,

Uzun süredir.Net Framework 4.0' ın bir parçası olarak gelen paralel programlama alt yapısı ile uğraşmıyordum. En son Beta 1 sürümündeyken Task Parallel Library ve PLINQ ile ilişkili konulara bakma fırsatım olmuştu. Zaman ilerledi ve.Net Framework 4.0 Beta 2 sürümü yayınlandı. Bu sürümde Beta 1' e göre bazı farklılıklar bulunmakta. Yani farklılıkları yeniden öğrenme aşamasına gelmiş durumdayız. Bunu WF tarafında, WCF tarafında gördüğümüz gibi halen gelişmekte olan Paralel programlama alt yapısında da görmekteyiz. İşte bu günkü yazımızda herhangibir Task'in iptal sürecinin Beta 2 sürümünde nasıl değerlendirildiğini incelemeye çalışıyoruz olacağız. Beta 1 sürümünde bir Task'in iptal edilmesi için aşağıdaki kod tasarımı kullanılmaktaydı.

```csharp
Task parallelTask= Task.Factory.StartNew(() =>
{
 for (; ; )
 {
  if (Task.Current.IsCancellationRequested)
  {
   Task.Current.AcknowledgeCancellation();
   return;
  }
  //TODO: Gerekli işlemler
 }
});
```

ve kodun herhangibir yerinde Task'in iptal edilmesi için ilgili Task nesne örneği üzerinden Cancel metodu çağırılmaktaydı.

```csharp
parallelTask.Cancel();
```

Bu modelde bir Task'in iptal istemi geldiğinde, çalışmakta olan güncel Task'in üzerinden IsCancellationRequested özelliğinin değerine bakılması gerekmekteydi. Eğer bu özelliğin değeri true ise bu durumda ilgili Task'in iptal işlemi ile ilişkili olaraktan bilgilendirilmesi amacıyla AcknowledgeCancellation metodu çağırılmakta ve örneğin return gibi bir çağrı ile paralel yüreyen operasyondan çıkılmaktaydı. Ancak Beta 1 sürümündeki bu yaklaşımın bazı handikapları da bulunmaktaydı. Söz gelimi iptal isteğinin kontrolü sırasında yürüyen süreci kesmek için return gibi bir çıkış kullanılması, Task tipi üzerinden static Current özelliğne gidilmek zorunda kalınması, iptal ile ilişkili tüm fonksiyonellik ve özelliklerin Task tipi üzerinde yer alması vb...Bu sebeplerden dolayı Beta 2 sürümünde bir Task'in iptal edilme işlemi yeniden değerlendirelerek daha tutarlı bir hale getirildi. Buna göre yeni modeli aşağıdaki örnekte görüldüğü üzere özetleyebiliriz.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskParallelLibrary
{
    public partial class Form1 : Form
    {
        CancellationTokenSource source;
        CancellationToken token;
        Task parallelTask;

        public Form1()
        {
            InitializeComponent();

            source = new CancellationTokenSource();
            token = source.Token;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            parallelTask = Task.Factory.StartNew(() =>
            {
                for (int i = 1000; i < 1000000; i++)
                {
                    // Eğer Cancel isteği gelirse OperationCancelled istisnası fırlatılır.
                    token.ThrowIfCancellationRequested();
                    // Burada bir takım işlemler yapılmakta olduğunu düşünebiliriz
                }
            }
            , token
            ); // StarNew metodunda kullanılan ikinci parametre ile CancellationToken referansının aktarıldığında dikkat edelim.
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            source.Cancel(); // İptal işlemi için Task referansı yerine CancellationTokenSource referansı kullanılmaktadır.
        }
    }
}
```

Beta 2 ile gelen iptal modelinde CancellationTokenSource, CancellationToken isimli yeni tiplerin kullanıldığı görülmektedir. Dikkat çekici noktalardan birisi, Task'in başlatılması işlemi sırasında ikinci parametre olarak bir CancellationToken referansının gönderilmesidir. Bu referanstan yararlanılarak for döngüsü içerisinde ThrowIfCancellationRequested metodu çağırılmaktadır. İşte bir yenilik daha. Bu metod, token referansı ile ilişkilendirilmiş olan Task'e bir iptal isteği gelip gelmediğini kontrol etmektedir. Eğer böyle bir istek gelmişse OperationCancelledException tipinden olan istisnayı fırlatmakta ve çalışmakta olan Task'in iptal edilmesine neden olmaktadır. Bir önceki modelde görüldüğü gibi bir if kontrolü yapılmasına ve return gibi bir çıkış yolu kullanılmasına gerek kalmamaktadır. Bir diğer önemli noktada, iptal işlemi için Task referansı yerine CancellationTokenSource nesne örneğinin kullanılıyor olmasıdır.

İptal işlemi ile ilişkili üyelerin tamamı Task tipinden çıkartılmıştır (Cancel,AcknowledgeCancellation,IsCancellationRequested,CurrentTask vb...) Cancel çağrısı için CancellationTokenSource, ThrowIfCancellationRequested çağrısı ile iptal kontrolü ve operasyonun kesilmesi için CancellationToken referanslarının kullanıldığına dikkat edelim. Buna göre bir iptal işlemi, aynı CancellationToken referansını kullanan birden fazla Task'ede uygulanabilir. Zaten bu amaçla, StartNew gibi bazı metodların yeni aşırı yüklenmiş versiyonlarına CancellationToken referansının taşınabiliyor olması sağlanmıştır.

Bakalım Beta 2 sürümüne göre paralel programlama alt yapısında başka ne gibi yenilikler bulunmaktadır. Bunları ilerleyen yazılarımızda incelemeye devam ediyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
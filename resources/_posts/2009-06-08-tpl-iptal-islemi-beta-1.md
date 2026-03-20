---
layout: post
title: "TPL - İptal İşlemi [Beta 1]"
date: 2009-06-08 13:19:00 +0300
categories:
  - tpl
tags:
  - tpl
  - csharp
  - windows-forms
  - task-parallel-library
  - threading
  - generics
---
Bir önceki blog yazımda, TPL kullanılarak WinForms uygulamalarında paralel işlemlerin nasıl yapılabileceğini ele almaya çalışmıştım. Örnekte son geldiğimiz noktaya bakıldığında aşağıdaki kazanımları elde ettiğimizi düşünebiliriz.

- Parallel.ForEach sayesinde resim dosyalarının iterasyonun daha hızlı gerçekleştirilebilmektedir.
- WinForms tarafındaki Cross-Thread ihlalinin önüne geçilmiştir.
- Task sınıfı üzerinden kullanılan StartNew metodu yardımıyla resim içeren Button kontrollerin üretildiği anda ekranda gösterilebilmesi sağlanmıştır.
- Yine StartNew metodunun kullanımı sayesinde kullanıcının paralel işlemler devam ederken, Form üzerindeki diğer kontroller ile etkileşimi sağlanmıştır.

Ancak biraz durup düşündüğümde unuttuğum önemli bir nokta olduğunu farkettim. İşlemler her ne kadar kısa gibi görünsede, kullanıcı bir yerde iptal etmek isterse ne olacak.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Dolayısıyla uygulamaya bir iptal sürecininde ekleniyor olması gerekmekte. Aslında bu işlem son derece basit. Nitekim Task sınıfının bu işlemler için tasarlanmış olan Cancel isimli bir metodu bulunmaktadır. Lakin örnek dikkatlice göz önüne alındığında, resim dosyları üzerindeki iterasyonun Parallel.ForEach yardımıyla gerçekleştirildiği görülür. Dolayısıyla Parallel.ForEach içerisinde hareket edilirken, iptal talebi gelip gelmediğinin kontrol edilmesi gerekmektedir. Ki buda tek başına yeterli değildir. Nitekim, Parallel.ForEach metoduda kendi içerisindeki işlemler için arka planda açtığı Task'leri kullanmaktadır. Dolayısıyla ForEach içeriğindeki task'lerinde iptal edilmesi gerekmektedir. Bu nedenle kod içeriğini aşağıdaki gibi değiştirmeliyiz.

```csharp
private Task task1 = null;

        private void btnStart4_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            Stopwatch watch = Stopwatch.StartNew();

            task1=Task.Factory.StartNew(() => FillImages(null));

            watch.Stop();
            lblElapsedTime.Text = String.Format("İşlemler {0} saniyede bitmiştir.", watch.Elapsed.TotalSeconds.ToString());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (task1 != null)
                task1.Cancel();
        }

        private void FillImages(object state)
        {
            Task currentTask = Task.Current;

            Parallel.ForEach(Directory.GetFiles(imagesPath), (f, ls) =>
            {
                if (currentTask.IsCancellationRequested)
                {
                    ls.Stop();
                    return;
                }
                FileInfo fInfo = new FileInfo(f);
                if (fInfo.Length <= 1024 * 100
                    && fInfo.Extension == ".jpg")
                {
                    Thread.Sleep(100); // Bunu koymadığımızda UI istediğimiz gibi reaksiyon vermiyor.
                    Button btn = new Button();
                    btn.Width = 64;
                    btn.Height = 48;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.BackgroundImage = Image.FromFile(f);
                    AddToPanel(btn);
                }
            }
            );
        }
```

Herşeyden önce iptal edilmek istenen Task sınıfına ait nesne örneğinin (task1 isimli değişken) sınıf seviyesinde bir değişken olarak ele alındığı görülmektedir. Kullanıcı iptal işlemi için Button kontrolüne tıkladığında, task1 değişkeni üzerinden Cancel metodu çağırılmaktadır. Bu durumda çalışma zamanında ForEach döngüsü içerisinde yer alan IsCancellationRequested özelliği true değeri döndürecektir. Bu özelliğe ulaşmak için Task sınıfının Current özelliğinden yararlandığımıza dikkat edilmelidir. Bu sayede ForEach içerisinde Parent Task örneğine ulaşılabilmektedir. Ardında ilginç bir kod parçası gelmektedir. ls isimli bir değişken üzerinden Stop metodu çağırılmıştır. İşte bu metod ForEach tarafından açılan task'lerin iptal edilmesini sağlamaktadır. Aslında Parallel sınıfı içerisinde ForEach veya For metodları içerisinde kullanılan temsilcilere bakıldığında ParallelLoopState isimli bir sınıf kullanıldığı görülür. Örneğin,

public static ParallelLoopResult ForEach (IEnumerable source, ActionParallelLoopState> body);

Bu sınıf içerisinde Stop, Break gibi paralel döngünün durdurulması veya dışına çıkılması için gerekli metodlar yer almaktadır. Örneğimizde Action temsilcisinin kullanıdığı ikinci generic parametre, çalışma zamanındaki ParallelLoopState nesne örneğine denk gelmektedir. Ve sonuç...

Tekrardan görüşmek dileğiyle hepinize mutlu günler dilerim.
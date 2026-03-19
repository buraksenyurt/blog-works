---
layout: post
title: "WF Rule Engine’ i Dışarıdan Kullanmak"
date: 2012-09-02 19:45:00 +0300
description: "Hangi sektörde olunursa olunsun, işler ister kağıt üstünde, ister elektronik ortamda yürüsün, iş süreçleri kendi içerisinde tanımlı bir çok kural kümesi içerir. İş bu kural kümeleri, gerekli durumlarda doğal yollarla sistemin bir parçası olarak ya da yürütme usulü ile manuel olarak devreye girerek, sürecin şekillenmesi ve bir takım aksiyonların alınması noktasında önemli rol üstlenirler. Biz geliştiricilerde, iş akışı mantığına dayalı sistemleri tasarladığımız durumlarda bu kural kümelerinin esnekliklerine sahip olmak isteriz. Bu Biztalk, Sharepoint, TIBCO vb iş akışı modellerini içeren gelişmiş ürünlerde çoğu zaman karşımıza çıkmaktadır."
categories:
  - wf
  - wf-4-0
tags:
  - wf
  - wf-4-0
  - csharp
  - xml
  - dotnet
  - workflow-foundation
  - windows-forms
  - xaml
  - http
  - authentication
  - serialization
  - visual-studio
  - rc
---
# WF Rule Engine’ i Dışarıdan Kullanmak
[![mfln2687l](/assets/images/2012/mfln2687l_thumb.jpg)](/assets/images/2012/mfln2687l.jpg) Merhaba Arkadaşlar,

Kurallar, kurallar, kurallar!Hayatın hemen her noktasında karşımıza tonlarca kural çıkar. Tabi mevzumuz kuralların zorlayıcılığı ve saire değil, kuralların ihlal edilmesi veya uyulması halinde gerçekleşen aksiyonların neler olduğu ile ilgilidir. Ki bu düşünce tarzı aslına bakarsanız iş dünyasının çekirdek süreçlerinden tutun, en ince ve hatta uç noktalarına kadar yayılır.

Hangi sektörde olunursa olunsun, işler ister kağıt üstünde, ister elektronik ortamda yürüsün, iş süreçleri kendi içerisinde tanımlı bir çok kural kümesi içerir. İş bu kural kümeleri, gerekli durumlarda doğal yollarla sistemin bir parçası olarak ya da yürütme usulü ile manuel olarak devreye girerek, sürecin şekillenmesi ve bir takım aksiyonların alınması noktasında önemli rol üstlenirler.

Biz geliştiricilerde, iş akışı mantığına dayalı sistemleri tasarladığımız durumlarda bu kural kümelerinin esnekliklerine sahip olmak isteriz. Bu Biztalk, Sharepoint, TIBCO vb iş akışı modellerini içeren gelişmiş ürünlerde çoğu zaman karşımıza çıkmaktadır.

Aslına bakarsınız uzun bir süre önce [Business Rule Engine ile Programlama (Biztalk Server 2006)](https://www.buraksenyurt.com/post/Business-Rule-Engine-ile-Programlama(Biztalk-Server-2006)) başlıklı bir makale yazmıştım. Bu makalede ana konu Biz Talk’ un gelişmiş Business Rule Engine ara birimini BizTalk ortamı dışında nasıl kullanabileceğimizi görmekti. O zamanlar bir POC (Proof of Concept) çalışması için yapmış olduğum araştırmanın sonuçlarının kayıt altına alınmış hali olan bu yazı pek çoğu gibi atıl oldu tabi...

Ancak nasıl ki Biztalk tarafında dışarıdan kullanabileceğimiz bir Rule Engine bulunmaktadır, benzer şekilde Workflow Foundation tarafında da kural setleri tanımlayıp, uygulatabileceğimiz bir çalışma zamanı motoru (Runtime Rule Engine) mevcuttur. İşte bu yazımızda söz konusu WF Rule Engine arabirimini herhangibir.Net uygulamasında basit anlamda nasıl kullanabileceğimizi bir kaç temel adımla görmeye/anlamaya çalışıyor olacağız.

Örnek senaryomuzda Product isimli bir POCO (Plain Old CLR Object) tip üzerinden kural tanımlanması, kuralların kayıt edilmesi, yeniden yüklenmesi ve istenildiği zaman canlı bir Product örneği üzerinde işletilmesi gibi fonksiyonellikleri sağlıyor olacağız. İşe ilk olarak aşağıdaki basit görünüme sahip olan bir Windows Forms uygulaması geliştirerek başlayabiliriz.

[![wfrule_Form](/assets/images/2012/wfrule_Form_thumb.png)](/assets/images/2012/wfrule_Form.png)

Test amaçlı olarak kullanacağımız bu Windows Form uygulamasında Product tipine ait basit kuralları tanımlayabileceğimiz, kayıt altına alabileceğimiz, tekrardan yükleyebileceğimiz ve çalıştırıp sonuçlarını görebileceğimiz işlevsellikler söz konusudur.

> Pek tabi Workflow Foundation Rule Set Engine kullanılmak istendiğinden, projeye aşağıdaki.Net Assembly’ larının da referans edilmesi gerekmektedir.[![wfrule_References](/assets/images/2012/wfrule_References_thumb.png)](/assets/images/2012/wfrule_References.png)

Şimdi arka planda gerekli olan kod üretimlerini gerçekleştirerek işlemlerimize devam edelim. Kural motoru için kullanacağımız Product tipi ve bir kuralın XAML (eXtensible Application Markup Language) olarak serileştirilmesi (Serialization) ile tekrardan geri yüklenmesi (DeSerialization) için gerekli fonksiyonellikleri içeren Utility sınıfının kod içerikleri aşağıdaki gibidir.

Sınıf diyagramı;

[![wfrule_Model](/assets/images/2012/wfrule_Model_thumb.png)](/assets/images/2012/wfrule_Model.png)

Product.cs;

```csharp
using System;

namespace WFRuleSetHowTo 
{ 
    public class Product 
    { 
        public Guid ProductId { get; set; } 
        public string Name { get; set; } 
        public decimal ListPrice { get; set; } 
        public int StockLevel { get; set; } 
       public string ErrorInformation { get; set; }

        public override string ToString() 
        { 
            return string.Format("{0} {1} {2} {3} [{4}]" 
                , ProductId 
                , Name 
                , ListPrice 
                , StockLevel 
                ,ErrorInformation 
                ); 
        } 
    } 
}
```

Utility.cs;

```csharp
using System; 
using System.Workflow.Activities.Rules; 
using System.Workflow.ComponentModel.Serialization; 
using System.Xml;

namespace WFRuleSetHowTo 
{ 
    public static class Utility 
    { 
        // Workflow RuleSet' lerin XAML bazlı serileştirilmesi/ters serileştirilmesi için gerekli nesne örneklenir 
        private static WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer(); 
        
        public static RuleSet Load(string ruleSetFileName) 
        { 
            RuleSet ruleSet = null; 
            try 
            { 
                // RuleSet dosyası okunmak üzere reader' a yüklenir 
                XmlTextReader reader = new XmlTextReader(ruleSetFileName); 
                // Ters serileştirme işlemi uygulanarak RuleSet içeriği nesnelleştirilir 
                ruleSet = serializer.Deserialize(reader) as RuleSet; 
                reader.Close(); 
            } 
            catch (Exception excp) 
            { 
                //TODO@Burak Do Something   
            }

            return ruleSet; 
        }

        public static bool Save(string ruleSetFileName, RuleSet ruleset) 
        { 
            bool result = false; 
            try 
            { 
                // RuleSet' i kaydetmek için bir writer oluşturulur 
                XmlTextWriter writer = new XmlTextWriter(ruleSetFileName, null); 
                // RuleSet ilgili dosya içerisine serileştirilir 
                serializer.Serialize(writer, ruleset); 
                result = true; 
                writer.Flush(); 
                writer.Close(); 
            } 
            catch (Exception excp) 
            { 
                //TODO@Burak Do Something 
            } 
            return result;         
        } 
    } 
}
```

Utility sınıfı temel olarak bir RuleSet örneğinin fiziki dosyaya XAML formatında serileştirilmesi veya tam tersi olarak XAML formatından geriye yüklenerek çalışma zamanında kullanılabilmesi işlevlerini barındırmaktadır. Product sınıfı ise örneğimizde kullanacağımız ve kural seti içerisinde ele alacağımız nesne şablonu olarak düşünülmelidir. Gelelim Form üzerindeki Button arkası kod parçalarına.

```csharp
using System; 
using System.IO; 
using System.Windows.Forms; 
using System.Workflow.Activities.Rules; 
using System.Workflow.Activities.Rules.Design;

namespace WFRuleSetHowTo 
{ 
    public partial class Form1 : Form 
    { 
        Product sampleProduct = null; 
        RuleSet sampleRuleSet = null; 
        string ruleSetFileName = Path.Combine(Environment.CurrentDirectory, "Product.rules");

        public Form1() 
        { 
            InitializeComponent(); 
            // Kullanılacak olan RuleSet örneklenir 
           sampleRuleSet = new RuleSet(); 
        }

       private void btnCreateProduct_Click(object sender, EventArgs e) 
        { 
            decimal price; 
            int stockLevel; 
            
            // RuleSet testi için örnek bir Product instance' ı oluşturulur 
           sampleProduct = new Product 
            { 
                ProductId=Guid.NewGuid(), 
                Name=!String.IsNullOrEmpty(txtProductName.Text)?txtProductName.Text:"Ornektir", 
                ListPrice=decimal.TryParse(txtProductListPrice.Text,out price)?price:1M, 
                StockLevel=int.TryParse(txtStockLevel.Text,out stockLevel)?stockLevel:100 
            };

            MessageBox.Show(string.Format("{0} örnek kullanım için üretildi",sampleProduct.ToString())); 
        }

        private void btnCreateRule_Click(object sender, EventArgs e) 
        { 
            // RuleSet' in oluşturulacağı Dialog nesnesi örneklenir 
            // ilk parametre kuralın uygulanacağı nesne tipidir 
            RuleSetDialog rsDialog = new RuleSetDialog(typeof(Product), null, sampleRuleSet); 
            if (rsDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            { 
               if (Utility.Save(ruleSetFileName,rsDialog.RuleSet)) 
                    MessageBox.Show("Rule Set başarılı bir şekilde kayıt edildi"); 
                else 
                    MessageBox.Show("İşlemlerinizi gözden geçiriniz. RuleSet kayıt edilemedi"); 
            } 
        }

        private void btnLoadRule_Click(object sender, EventArgs e) 
        { 
            sampleRuleSet = Utility.Load(ruleSetFileName); 
            if (sampleRuleSet != null) 
                MessageBox.Show("RuleSet başarılı bir şekilde yüklendi"); 
            else 
                MessageBox.Show("RuleSet yüklenemedi!"); 
        }

        private void btnRunRule_Click(object sender, EventArgs e) 
        { 
            // Elimizde bir RuleSet' imiz var ise 
            if (sampleRuleSet != null) 
            { 
                // Kuralı işletmek için gerekli doğrulama nesnesi örnek Product tipi için üretilir 
                RuleValidation validation = new RuleValidation(sampleProduct.GetType(), null); 
                // Kuralı işletecek olan motor örneklenir. İlk parametre doğrulama kriterlerini ikinci parametre ise doğrulamaya tabi olacak canlı nesne örneğini içerir 
                RuleExecution engine = new RuleExecution(validation, sampleProduct); 
                // Kural işletilir. 
                sampleRuleSet.Execute(engine); 
                MessageBox.Show(sampleProduct.ToString()); 
            } 
        } 
    } 
}
```

Geliştirici bu arabirim üzerinden bir Product tipi için yeni RuleSet tanımlayabilir, kayıt altına alabilir, kayıtlı olanı yükleyebilir ve işletebilir. İşin temelinde RuleSetDialog, RuleValidation, RuleExecution ve RuleSet tipleri yer almaktadır.

RuleSet nesne örneğine ait Execute metodu parametre olarak gelen RuleExecution instance’ ını baz alarak bir kural kümesi işletimini gerçekleştirmektedir. RuleExecution, hangi nesne örneği için ilgili kural kümesinin çalıştırılacağını, ikinci parametresi sayesinde bilmekte olup ilk parametre ile de bir doğrulama işlemini sürece dahil etmektedir. Bu doğrulama, RuleValidation örneğine göre bir.Net tipi için (örneğimizde Product sınıfıdır) icra edilmektedir.

RuleSetDialog tipi ile aşağıdakine benzer bir iletişim kutucuğunun (Rule Set Editor) çalışma zamanında açılması ve yine resimde görüldüğü gibi örnek bir kuralın tanımlanması mümkündür.

[![wfrule_dialog](/assets/images/2012/wfrule_dialog_thumb.png)](/assets/images/2012/wfrule_dialog.png)

Örnekte tanımlanan StockLevelRule ile, herhangibir Product nesne örneğinin StockLevel değerinin 250’ nin altında olması hali ele alınmış ve durumun true veya false olmasına göre yine o anki canlı Product nesne örneğinin ErrorInformation özelliğine bazı bilgilendirme mesajları atanmıştır. (Çok doğal olarak burada başka atamaların yapılması da söz konusu olabilir) Tanımlanan bu kural seti serileştirilerek kayıt altına alındığında ise aşağıdaki XAML içeriğinin üretildiği görülür.

```xml
<RuleSet Description="{p1:Null}" Name="{p1:Null}" ChainingBehavior="Full" xmlns:p1="http://schemas.microsoft.com/winfx/2006/xaml" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/workflow"> 
    <RuleSet.Rules> 
        <Rule Priority="0" ReevaluationBehavior="Always" Description="{p1:Null}" Active="True" Name="StockLevelRule"> 
            <Rule.Condition> 
                <RuleExpressionCondition Name="{p1:Null}"> 
                    <RuleExpressionCondition.Expression> 
                        <ns0:CodeBinaryOperatorExpression Operator="LessThan" xmlns:ns0="clr-namespace:System.CodeDom;Assembly=System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"> 
                            <ns0:CodeBinaryOperatorExpression.Right> 
                                <ns0:CodePrimitiveExpression> 
                                    <ns0:CodePrimitiveExpression.Value> 
                                        <ns1:Int32 xmlns:ns1="clr-namespace:System;Assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089">250</ns1:Int32> 
                                    </ns0:CodePrimitiveExpression.Value> 
                                </ns0:CodePrimitiveExpression> 
                            </ns0:CodeBinaryOperatorExpression.Right> 
                            <ns0:CodeBinaryOperatorExpression.Left> 
                                <ns0:CodePropertyReferenceExpression PropertyName="StockLevel"> 
                                    <ns0:CodePropertyReferenceExpression.TargetObject> 
                                        <ns0:CodeThisReferenceExpression /> 
                                    </ns0:CodePropertyReferenceExpression.TargetObject> 
                                </ns0:CodePropertyReferenceExpression> 
                            </ns0:CodeBinaryOperatorExpression.Left> 
                        </ns0:CodeBinaryOperatorExpression> 
                    </RuleExpressionCondition.Expression> 
                </RuleExpressionCondition> 
            </Rule.Condition> 
            <Rule.ThenActions> 
                <RuleStatementAction> 
                    <RuleStatementAction.CodeDomStatement> 
                        <ns0:CodeAssignStatement LinePragma="{p1:Null}" xmlns:ns0="clr-namespace:System.CodeDom;Assembly=System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"> 
                            <ns0:CodeAssignStatement.Left> 
                                <ns0:CodePropertyReferenceExpression PropertyName="ErrorInformation"> 
                                    <ns0:CodePropertyReferenceExpression.TargetObject> 
                                        <ns0:CodeThisReferenceExpression /> 
                                    </ns0:CodePropertyReferenceExpression.TargetObject> 
                                </ns0:CodePropertyReferenceExpression> 
                            </ns0:CodeAssignStatement.Left> 
                            <ns0:CodeAssignStatement.Right> 
                                <ns0:CodePrimitiveExpression> 
                                    <ns0:CodePrimitiveExpression.Value> 
                                        <ns1:String xmlns:ns1="clr-namespace:System;Assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089">Stok seviyesi kritik</ns1:String> 
                                    </ns0:CodePrimitiveExpression.Value> 
                                </ns0:CodePrimitiveExpression> 
                            </ns0:CodeAssignStatement.Right> 
                        </ns0:CodeAssignStatement> 
                    </RuleStatementAction.CodeDomStatement> 
                </RuleStatementAction> 
            </Rule.ThenActions> 
            <Rule.ElseActions> 
                <RuleStatementAction> 
                    <RuleStatementAction.CodeDomStatement> 
                        <ns0:CodeAssignStatement LinePragma="{p1:Null}" xmlns:ns0="clr-namespace:System.CodeDom;Assembly=System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"> 
                            <ns0:CodeAssignStatement.Left> 
                                <ns0:CodePropertyReferenceExpression PropertyName="ErrorInformation"> 
                                    <ns0:CodePropertyReferenceExpression.TargetObject> 
                                        <ns0:CodeThisReferenceExpression /> 
                                    </ns0:CodePropertyReferenceExpression.TargetObject> 
                                </ns0:CodePropertyReferenceExpression> 
                            </ns0:CodeAssignStatement.Left> 
                            <ns0:CodeAssignStatement.Right> 
                                <ns0:CodePrimitiveExpression> 
                                    <ns0:CodePrimitiveExpression.Value> 
                                        <ns1:String xmlns:ns1="clr-namespace:System;Assembly=mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089">Stok seviyesi normal</ns1:String> 
                                    </ns0:CodePrimitiveExpression.Value> 
                                </ns0:CodePrimitiveExpression> 
                            </ns0:CodeAssignStatement.Right> 
                        </ns0:CodeAssignStatement> 
                    </RuleStatementAction.CodeDomStatement> 
                </RuleStatementAction> 
            </Rule.ElseActions> 
        </Rule> 
    </RuleSet.Rules> 
</RuleSet>
```

Her ne kadar bu çıktıyı gözle takip etmek zor olsa da şu noktaya dikkat edilmelidir.

> XAML olarak üretilen içerik Notepad gibi basit bir metin editörü ile açılıp düzenlenebilir. Bir başka deyişle kuralların dekleratif olarak tanımlanabilmesi, güncellenmesi ve devreye alınması söz konusudur.

Pek tabi kayıt altına serileştirerek almış olduğumuz bu XAML içeriğini uygulamayı kapatsak bile tekrardan aynı veya farklı uygulamalara yükleyebilir ve işletebiliriz. Uygulamamızda örnek bir Product için kural çalıştırıldığında aşağıdaki sonucun alındığı gözlemlenir.

Stok seviyesinin kuralda tanımlanan 250 birimin altında olması halinde,

[![wfrule_Run1](/assets/images/2012/wfrule_Run1_thumb.png)](/assets/images/2012/wfrule_Run1.png)

Stok seviyesinin kuralda tanımlanan 250 birimin üstünde olması halinde,

[![wfrule_Run2](/assets/images/2012/wfrule_Run2_thumb.png)](/assets/images/2012/wfrule_Run2.png)

Görüldüğü gibi Workflow Foundation ile birlikte gelen kural motorunun herhangibir.Net uygulaması üzerinden kullanılabilmesi son derece kolaydır. Hatta bu tip bir arabirim yardımıyla, iş analistlerinin çeşitli kurallar tanımlayıp kayıt altına alabilecekleri ve aslında süreç yönetim araçlarında önemli yere sahip olan bir takım depolama programlarının geliştirilmesi de kolaylaşmaktadır. Çok doğal olarak bu kurallar bir servis arkasında işletilebilirler de.

> WF Rule Set Editor arayüzü, kullanıcısına daha esnek bir şekilde kural tanımlayabilme ve bunları kayıt altına alarak saklayabilme imkanı sunmaktadır.

> XAML formatlı olarak kayıt altına alınabilen kural kümeleri (RuleSet) istenildiği zaman çalışma zamanına yüklenebilir ve tanımın ait olduğu nesne örneği/örnekleri için işletilebilir.

> Rule Set Editör içerisinde birden fazla kural tanımlanabilir ve bunlar çalışma zamanında yürütülebilir.

Bu yazımızda çok basit olarak Workfow Rule Engine alt yapısına bir Merhaba demeye çalıştık. Kapıyı aralamak benden içeri girip yürümek ise sizden. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek her nedense halen RC sürümünde olan Visual Studio 2012 ile geliştirilmiştir. Ancak Visual Studio 2010 ile de çalışmaktadır]

[WFRuleSetHowTo.zip (60,57 kb)](/assets/files/2012/WFRuleSetHowTo.zip)
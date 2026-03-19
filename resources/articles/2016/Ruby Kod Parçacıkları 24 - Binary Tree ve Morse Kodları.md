---
title: "Ruby Kod Parçacıkları 24 - Binary Tree ve Morse Kodları"
pubDate: 2016-11-06 21:30:00
categories:
  - Ruby
tags:
  - Ruby
---

# Ruby Kod Parçacıkları 24 - Binary Tree ve Morse Kodları
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Geçtiğimiz günlerde çalışma arkadaşımla aramızda gelişen teknolojiler üzerine bir takım konuşmalar geçti. Amazon Alexa'dır, Facebook'un Face Recognation teknolojileridir, Arduino'dur, Drone'lardır vs derken OCR konusuna da değindik. Bükük duran bir kağıdın fotoğrafını çeken uygulamanın, kağıt üzerindeki şekli düz bir biçimde algılayabilidiğini, bunu yapmak içinse matematik'teki Laplace denklemleri ve Fourier serilerine başvurduğunu öğrendik. Bir Matematik Mühendisi olarak bu dersleri hep teorik olarak okumuştum. Tabii o zamanlarda bu tip denklemlerin gerçek hayat uygulamalarını pek görememiştim. Bu yüzden çoğu teori soyut olarak kalmıştı.

Benzer durumu Ruby ile ilgili çalışmalarıma devam ederken de yaşadım. Geçtiğimiz günlerde Enumerable modülünü incelemeye çalışırken kendimi bir anda Binary Tree veri yapısını tekrardan incelerken buldum. C# tarafında bu veri yapısını incelemiş ve kullanmıştım ancak Ruby tarafında incelerken farklı bir yaklaşımı benimsedim. Aynen OCR denklemlerinde olduğu gibi bunu gerçek hayat örnekleri ile eşleştirmeye gayret ettim. Yaptığım araştırmalar sonuncunda Morse kodlarının ikili ağaç yapısı ile ifade edilebildiğini öğrendim. Bilmediğim bir bilgiydi ve Binary Tree'yi daha iyi anlamama yardımcı oldu.

Binary Tree'ler aslında bir veri yapısı (Data Structure) olarak ifade ediliyor. Bu veri yapısı birbirine bağlı boğumlardan (Node) oluşmakta. Her boğum iki alt boğum içermekte ve sağ sol olmak üzere en fazla iki alt dala ayrılmakta. Solda veya sağda kalan boğumlar kendi aralarında benzer ilişkilere sahip olacak şekilde bir dizilim söz konusu. Ne demek istediğimi daha iyi anlatabilmek için Morse kodlarının ağaç yapısında nasıl ifade edildiğine bir bakalım. Bu amaçla aşağıdaki grafiği göz önüne alabiliriz.

![image.axd](images/image.axd)

Root'tan başlayarak aşağıya doğru dallanan ve morse alfabesindeki her bir değerin boğumlar içerisinde ele alındığı bir ağaç yapısı söz konusu. Sol dalda yer alan boğumlar arasındaki ilişkiler nokta (.), sağ boğumlar arasındaki ilişkiler ise tire (-) sembolü ile ifade ediliyor. Yani kısa ve uzun sinyal olarak ifade edebiliriz. Buna göre bir harfin veya sayının Morse kodunu bulmak için ağaç üzerinde o değere doğru ilerlememiz yeterli. Söz gelimi 0'ın Morse kodu beş uzun sinyalden oluşuyor (-----) Yani T->M->O->Boşluk->0 sırasını izlersek kodu bulabiliriz. A harfini göz önüne aldığımızda bir kısa bir uzun sinyal söz konusu (.-) Buna göre root->E->A şeklinde ilerlememiz yeterli.

Binary Tree çok hızlı bir veri yapısı olmasa da çok yavaş da değil ve söz konusu Morse kodlarına uygun bir model sunmakta. Üzerinde arama kriterleri de uygulandığında tadından yenmeyecek bir veri içeriği oluşuyor.

Peki Ruby tarafında bu tip bir veri yapısını nasıl oluşturabiliriz? Bunun bir kaç yolu var ama birbirine bağlı boğumları tanımlama ve ağacı oluşturmak şu an daha ilgi çekici geliyor. Benim için iki ana unsur var. Birincisi boğumun içerisindeki harfi ve mors kodunu temsil edecek bir sınıfa ihtiyacım olacak. Diğer yandan Node yapısını işaret edecek bir sınıf daha gerekiyor. Bu sınıfa Enumerable modülünü dahil ederek kendi üzerinde taşıdığı öğeler üzerinde iteratif hareket edebilmeyi umuyoroum. İlk olarak Code sınıfını tanımlayalım.

```text
class Code
	attr_accessor :letter, :signal
	def initialize(letter,signal)
		@letter=letter
		@signal=signal
	end
	def to_s
		"#{@letter}-#{@signal}"
	end
end
```

Code sınıfı bir harf veya sayıya karşılık gelecek sinyali barındırmakta. Node sınıfını ise temel olarak aşağıdaki gibi tasarlayabiliriz.

```text
class Node
	include Enumerable
	
	attr_accessor :owner,:left,:right
	
	def initialize(letter,signal)
		@owner=Code.new(letter,signal)
	end
	
	def each(&block)
		left.each(&block) if left
		block.call(self)
		right.each(&block) if right
	end
	
	def translate(word)
		newWord=""
		word.split("").each{|c|newWord<<find{|n|n.owner.letter==c.upcase}.owner.signal}
		newWord.chomp
	end
	
	def to_s
		@owner.to_s
	end
end
```

Node sınıfı bir boğumun kendisi ve bu boğumun sağ ile sol boğumlarını temsil eden sınıfımızdır. Initialize metoduna gelen iki parametre ile harfi ve sinyali alıp bir Code nesne örneği oluşturmaktadır. Bünyesinde barındırdığı each metodunun dikkat çekici yanı parametre olarak bir block almasıdır. Bu bloğu kendisine, varsa sol ve sağ boğumlarına da uygular. left ve right değişkenleri üzerinden de each çağrısı yapıldığına dikkat edelim. each metoduan gelen kod bloğu bu sayede left ve right değişkenleri altındaki boğumlara da uygulanmaktadır. Kısacası Node örneğine ait fonksiyona bir kod bloğunu gönderebilir ve boğumun kendisi ile sonrasında gelen elemanlar için bu blok içeriğini çalıştırabiliriz. Örneğin root ve altındaki tüm boğumları yazdırabiliriz. Bunun için aşağıdaki kodu kullanmak yeterli.

```text
root.each{|n|puts n.to_s}
```

![image.axd](images/image.axd)

Ekran görüntüsünde görüldüğü gibi tüm boğumları elde ettik. Bu arada bu boğumları nasıl oluşturduk diye düşünebilirsiniz. Tekniğinden tam olarak emin olamadığım biraz da amelece diyebileceğimiz bir kodlama yapmak durumunda kaldım. Morse kodlarına ait ikili ağaç yapısını Node sınıfına ait bir nesne örneği üzerinden tek tek ekleyerek oluşturmaya çalıştım. Kodun uzunluğu için şimdiden beni mazur görün. Eminim çok daha şık bir yol vardır. Pişmanım ama elden ne gelir.

```text
root=Node.new("root",nil)
Empty1=Node.new(nil,nil)
Empty2=Node.new(nil,nil)
Empty3=Node.new(nil,nil)
E=Node.new("E",".")
I=Node.new("I","..")
S=Node.new("S","...")
H=Node.new("H","....")
V=Node.new("V","...-")
U=Node.new("U","..-")
F=Node.new("F","..-.")
A=Node.new("A",".-")
R=Node.new("R",".-.")
L=Node.new("L",".-..")
W=Node.new("W",".--")
P=Node.new("P",".--.")
J=Node.new("J",".---")
T=Node.new("T","-")
N=Node.new("N","-.")
M=Node.new("M","--")
G=Node.new("G","--.")
O=Node.new("O","---")
D=Node.new("D","-..")
K=Node.new("K","-.-")
B=Node.new("B","-...")
X=Node.new("X","-..-")
C=Node.new("C","-.-.")
Y=Node.new("Y","-.--")
Z=Node.new("Z","--..")
Q=Node.new("Q","--.-")

N0=Node.new("0","-----")
N1=Node.new("1",".----")
N2=Node.new("2","..---")
N3=Node.new("3","...--")
N4=Node.new("4","....-")
N5=Node.new("5",".....")
N6=Node.new("6","-....")
N7=Node.new("7","--...")
N8=Node.new("8","---..")
N9=Node.new("9","----.")

root.left=E
root.right=T
E.left=I
E.right=A
I.left=S
I.right=U
U.left=F
U.right=Empty1
Empty1.right=N2
S.left=H
H.left=N5
H.right=N4
S.right=V
V.left=N3
A.left=R
R.left=L
A.right=W
W.left=P
W.right=J
J.right=N1
T.left=N
T.right=M
N.left=D
N.right=K
D.left=B
D.right=X
B.left=N6
K.left=C
K.right=Y
M.left=G
M.right=O
G.left=Z
Z.left=N7
G.right=Q
O.left=Empty2
O.right=Empty3
Empty2.left=N8
Empty3.left=N9
Empty3.right=N0
```

Buna göre herhangibir boğum ve sonrasında gelenleri each metodu üzerinden çekebiliriz. Örneğin kök boğumun sağ tarafındaki diziden G ve altındaki bağlantıları bulalım.

```text
G.each{|n|puts n.to_s}
```

![image.axd](images/image.axd)

Translate fonksiyonu ise bir kelimenin harflerini tek tek ele alıp find metodu üzerinden hareket ederek karşılık gelen sinyalleri bulmak için yazılmıştır. Aşağıdaki kod parçasında bir kaç örnek kullanımını bulabilirsiniz.

```text
signs=["WeAreSinkingStop","soss","HelpUSStop"]
signs.each{|w| puts "#{w} = #{root.translate(w)}"}
```

![image.axd](images/image.axd)

Görüldüğü gibi yazılan metinlerin Morse kodlarına göre karşılıklarını elde ettik. Aslında bir harfin karşılığı olan morse kodunun key-value çiftleri şeklinde tutulması da mümkün. Bizim buradaki amacımız ikili ağaç veri yapısında bu serinin nasıl oluşturulabileceğini görmekti. Diğer yandan kodda dikkat çekici başka noktalar da bulunmakta. Enumerable modülünün dahil edilmesi, each metoduna block geçirilmesi ve find operasyonunun ezilmesi bunlar arasında sayılabilir. Bir antrenman kodu olarak değerlendirebilirsiniz. Tabii kodda bir çok sıkıntı var. Söz gelimi ikili ağaç resminde görünün sırada elde edemiyoruz. Buna bir bakmak lazım. Böylece geldik bir Ruby maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

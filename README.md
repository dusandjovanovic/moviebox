# moviebox
Universal Windows Application for getting and storing movie and series metadata. Neo4j graph database integration.

MovieBox  je aplikacija koji omogućava organizovanje i lako održavanje kolekcije filmova. Održavanje kolekcije podrazumeva dodavanje, modifikaciju i brisanje filmova. Pored filmova postoji i pregled sezona serija. MovieBox omogućava nalaženje odredjenog filma iz kolekcije i reprodukciju sadržaja ukoliko je taj konkretan film prisutan na fajlsistemu korisnika. Pored održavanje biblioteke MovieBox pruža i pribavljanje preporučenih filmova, formiranih na osnovu trenutnog sadržaja biblioteke.

### Graph baza Neo4j
Za održavanje perzistencije koristi se neo4j baza podataka u koju se preslikava model podataka prisutan u samoj logici aplikacije.
Neophodno je pokrenuti neo4j runtime iz foldera **neo4j/bin/** komandom `neo4j console`. Ne treba zatvarati konekciju prilikom korišćenja aplikacije.

`username: neo4j`

`password: admin`

Za pristupanje bazi podataka koristi se [neo4jclient](https://github.com/Readify/Neo4jClient).

#### Struktuiranost graph baze

![alt text][movieGraph]

[layered]: metadata/graph.png

![alt text][seriesGraph]

[layered]: metadata/graph(1).png

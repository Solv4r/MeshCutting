Projektname: 
	Time shifting mechanic with dynamic mesh cutting

Namen der Teammitglieder:
	Rafael Sekula
	Kai Mittendorff

Besonderheiten des Projekts:
	Zum Starten des Projekts PresentFutureTesting Szene öffnen und starten. Es kann sein, dass das Material bei der Treppe auf der rechten Seite fehlt. Ist das der Fall dann einfach unter Assets/materials/Used Materials in Map/Shader2
	auf Basemap/LowerLevels/Right Stair Case Base sowie Left&Right boarder drag'n droppen
	Character mit WASD steuern (shift für sprint)
	Um Scheinwerfer an andere Position zu steuern in die Nähe laufen und mit "E" umschalten
	Um Zeit zu wechseln zum Podest mit Buch laufen und in der Nähe "F" betätigen (2 sek cooldown)
	Wenn die Zeit gewechselt wird sind Objekte nur im Scheinwerfer sichtbar und wenn wieder zurück gewechselt wird sind sie wieder nur außerhalb des Lichtkegels sichtbar
	Objekte sollen sowohl visuell als auch physisch geschnitten werden damit neue Objekte entstehen und man durch "halbe" Objekte hindurchgehen kann 

Besondere Leistungen, Herausforderungen und gesammelte Erfahrungen während des Projekts. Was hat die meiste Zeit gekostet?

MeshCutting: Die größste Herausforderungen initial war es, zu verstehen, wie Objekte in Unity aufgebaut sind, bevor man überhaupt daran denken konnte, diese zu schneiden. 
Nach anfänglicher Recherche und Inspiration aus verschiedenen Quellen, haben wir uns entschieden an einem Ansatz zu arbeiten, bei der man ein Objekt durch eine Plane schneidet, die unendlich groß ist. 
Dabei wird für jeden Eckpunkt eines Objekten geschaut, ob er sich vor oder hinter der Plane befindet, indem man sich die Distanz eines Eckpunktes zur Plane holt. 
Positive Werte sind vor der Plane und negative Werte bedeutet, das sich Eckpunkte hinter der Plane befinden und demnach geschnitten werden sollen.
Dieser Ansatz hat aber weitere Probleme mit sich gebracht. Die Darstellung des Meshes konnten wir nie richtig fixen, da Dreiecke des Meshes in einer falschen Reihenfolge aufgebaut wurden. 
Dies war aber nur ein bedingtes Problem, da die visuelle Darstellung sowieso über einer Shader gehandhabt wurde und für das Meshcutting nur der geschnittene Collider von Bedeutung ist. 
Nachdem es uns möglich war Objekte durch eine Plane zu schneiden, ging es darum, das Script fähig zu machen, Objekte in einer Kegelform zu schneiden.
Der gewählte Ansatz hierbei ist, dass man sich für die Anzahl der Segmente des Kegels einzelne Planes in einer Kegelform erstellt und durch diese iteriert und den gleichen Algorithmus verwendet. 
Für das Schneiden und das Anzeigen von Objekten außerhalb des Lichtkegels funktionierte dieser Ansatz ohne weitere Schwierigkeiten. Problematisch wurde es aber dann, wenn man Objekte innerhalb des Lichtkegels schneiden und anzeigen lassen wollte.
Man könnte meinen, dass man nur die Logik umdrehen muss, aber so einfach war es leider nicht.
Dadurch, dass wir für jede der Planes schauen, ob ein Eckpunkt eines Objektes positiv oder negativ zur Plane ist, ist es passiert, dass ein negativer Punkt der ganz linken Plane des Kegels wiederum positiv für die ganz rechte Plane sein kann. 
Dies führte dazu, dass Meshes völlig falsch geschnitten und dann dargestellt wurden. Lösung hierbei war es, jeden Eckpunkt mit jeder Plane zu prüfen, und nur zum neuen Mesh hinzuzufügen, wenn dieser für alle Planes negativ ist.

Cutout Shader (per Untiy Shadergraph erstellt):

4 Parameter als Eingaben:
ConeOrigin (Kegelspitze)
ConeDirection (Richtung des Kegels)
ConeAngle (Öffnungswinkel)
ConeRange maximale Reichweite des Kegels

Ist grundsätzlich in 2 Teile Aufgebaut: 
	1. Berechnung ob ein Punkt im Kegel liegt
	2. Transparenz & Clipping (Punkt zeichnen oder ausblenden)

Cutout shader sind alle auf dem gleichen Prinzip aufgebaut:
	1. Teil:
		1 Position -> 3D-Koordinate der aktuellen Oberfläche
		2 Subtract -> Vektor von position - ConeOrigin
		3 Length -> prüfen, ob Punkt innerhalb der Reichweite liegt
		4 Richtungsvektoren Normalisieren 
		5 Dot Product Winkelvergleich zwischen Richtungsvektor zum Pixel und Kegelrichtung (größerer Wert = näher am Zentrum des Kegels)
		6 Referenzwert mit Cosinus berechnen -> Grenzwert ab wann Pixel innerhalb des Kegelwinkels liegt
		7 Die 2 Bedingungen vergleichen (Punkt NAH genug? & im richtigen Winkelbereich?)
		8 AND node kombiniert beide Bedingungen
	2. Teil: 
		1 Branch Node -> Wenn er im Kegel liegt Alpha 0 (unischtbar) sonst Alpha 1 (sichtbar)
		2 Alpha Clip Threshhold -> entfernt alle Pixel deren Alpha keliner ist als der Schwellenwert
		3 Andere Parameter (Color, Smoothness usw) steuern Aussehen 
	Zum cutten von den Schalen auf der Treppe wird beim gleichen Shader noch eine Farbe per Farbnode hinzugefügt & für die Treppe wird zusätzlich noch ein Tiling Parameter (für die Textur) und die Textur selbst in den Farbkanal gegeben (siehe Cutout Shader BOOLEAN 2)

Verwendete Assets, Codefragmente, Inspiration. Alles was Sie nicht selbst gemacht haben bitte unbedingt angeben.

YughuesFreeMetalMaterials: 	https://assetstore.unity.com/packages/2d/textures-materials/metals/yughues-free-metal-materials-12949
YughuesFreePavementMaterials:	https://assetstore.unity.com/packages/2d/textures-materials/roads/yughues-free-pavement-materials-12952
Inspiration Meshcutting: https://www.youtube.com/watch?v=BVCNDUcnE1o
Meshcutting mit einer Plane die durch ein Objekt schneidet: https://github.com/ElasticSea/VR-Slice
CharacterModell: Diese Modelle werden hier vom Entwickler für fair use zur Verfügung gestellt: https://www.aplaybox.com/u/516827875 (Chinesische Webseite, Login notwendig)
(Von Fans zur Verfügung gestellte Download Links zu den Modellen: https://www.reddit.com/r/HonkaiStarRail/comments/132dshx/honkai_star_rail_official_mmd_models/ https://drive.google.com/drive/u/0/folders/1UZnuFJZZqiYws2geifYxAZI3a98abl6x)
Character Shader: https://github.com/ColinLeung-NiloCat/UnityURPToonLitShaderExample (Example Version die hier zur Verfügung gestellt wird)
Animationen: ThirdPersonController Starter Asset, welches wir auch in der Vorlesung benutzt haben

Eigens erstellte Assets 
Keine

Link zu einem kurzen Video (ca. 60-120s), welches das Projekt und seine Features in Action zeigt
Video Link: https://youtu.be/0HRrqYYI0xE

Link zum Projekt falls zu groß für Upload in Ilias
Repo Link: https://github.com/Solv4r/MeshCutting (Main branch als zip runterladen)

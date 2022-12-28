#Générateur de mot de passe aléatoire

import turtle
import random

minuscules ="abcdefghijklmnopqrstuvwxyz"
majuscules="ABCDEFGHIJKLMNOPQRSTUVWXYZ"
chiffres="0123456789"
symboles="&')(@=+*/ç,"

utilisation = minuscules + majuscules + chiffres + symboles
longueur = 10

mot_de_passe = "".join(random.sample(utilisation,longueur))

print("Votre mot de passe généré est : ")
print(mot_de_passe)


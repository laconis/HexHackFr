import mysql.connector
import re

# Expression régulière pour vérifier un hash SHA-256 (64 hex)
SHA256_REGEX = re.compile(r"^[a-fA-F0-9]{64}$")

# Connexion MySQL
db = mysql.connector.connect(
    host="localhost",
    user="root",
    password="motdepasse",
    database="ma_base"
)

cursor = db.cursor()

# Récupération des hashes
cursor.execute("SELECT id, hash_sha256 FROM utilisateurs")
rows = cursor.fetchall()

valid = 0
invalid = 0
duplicates = {}

for row in rows:
    user_id, hash_value = row

    # Vérification du format SHA-256
    if SHA256_REGEX.match(hash_value):
        valid += 1
    else:
        invalid += 1
        print(f"[!] Hash invalide pour ID {user_id}: {hash_value}")

    # Détection de doublons
    duplicates.setdefault(hash_value, 0)
    duplicates[hash_value] += 1

# Affichage des doublons
print("\n=== Doublons détectés ===")
for h, count in duplicates.items():
    if count > 1:
        print(f"{h} apparaît {count} fois")

print("\n=== Résumé ===")
print(f"Hashes valides   : {valid}")
print(f"Hashes invalides : {invalid}")

cursor.close()
db.close()













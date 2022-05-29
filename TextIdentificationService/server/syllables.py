import json

class Syllables:
    @staticmethod
    def getSyllables(wordToFind):
        f = open(r'C:\Projetos\Mestrado\Project II\SourceCode\TextIdentificationService\datasets\syllables\syllable_word.json')
        data = json.load(f)

        syllables = []

        for word in data['words']:
            if(word['word'] == wordToFind):
                if(word['syllable'] == "No syllable"):
                    syllables = [wordToFind]
                else:
                    syllables = word['syllable'].upper().split("-")
                break

        f.close()
        return syllables

print(Syllables.getSyllables("ability"))

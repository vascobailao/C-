# Makefile de exemplo

parshell: list.o commandlinereader.o parshell.o fibonacci.o exemplo.o
	gcc -pthread -o parshell list.o commandlinereader.o  exemplo.o parshell.o 

fibonacci.o: fibonacci.c
	gcc  -o fibonacci fibonacci.c

commandlinereader.o: commandlinereader.c commandlinereader.h
	gcc -Wall -g -c commandlinereader.c

parshell.o: parshell.c commandlinereader.h 
	gcc -Wall -g -c parshell.c

list.o: list.c list.h
	gcc -Wall -g -c list.c

exemplo.o: exemplo.c exemplo.h
	gcc -Wall -g -c exemplo.c

clean:
	rm -f *.o parshell fibonacci


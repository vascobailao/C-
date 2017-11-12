
// Par-shell terminal
//

#include <stdlib.h>
#include <sys/wait.h>
#include <string.h>
#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <pthread.h>
#include "list.h"
#include "exemplo.h"
#include "semaphore.h"
#include <unistd.h>
#include <fcntl.h>

extern int status;
extern int nChilds;
extern list_t *list;
extern int exit1;
extern pthread_mutex_t lock;
extern FILE *fptr;
extern pthread_cond_t semfilhos;
extern pthread_cond_t semexec;
extern int totaltime;
extern int contaiteracoes;


#define SIZE 5
#define MAX_BUFER 1024


int main (int argc, char** argv){

    char *argvector[SIZE];


    int fd;
    char * myfifo = "/tmp/parshell-in";
    char arg[64];
    
    /* create the FIFO (named pipe) */
    
    if(access(myfifo, F_OK) == -1 )
        if(mkfifo(myfifo, 0666)<0)
            exit(-1);
    
    /* write "Hi" to the FIFO */
    fd = open(myfifo, O_WRONLY);


    snprintf(arg, 64, "Processpid %d\n", (int) getpid());
    write(fd, arg, strlen(arg)); 

    char sitio[MAX_BUFER];
    int fd2;
    char * myfifo2 = "/tmp/parshell-in1";
    fd2 = open(myfifo2, O_RDONLY);




    while(1) {

        fgets(arg, sizeof(arg), stdin);



        if(strcmp(arg,"exit")==0){
            return(EXIT_SUCCESS);
        }

        if(strncmp(arg,"stats",5)==0){

            write(fd, arg, strlen(arg));


            
            read(fd2, sitio, strlen(sitio));
            printf("%s\n", sitio);

            printf("Entrei no stats1\n");


    

            //printf("Entrei no stats2\n");
            
            printf("%lu\n", strlen(sitio));
            //printf("Entrei no stats3\n");

            //printf("%s\n", bufer);

            printf("Entrei no stats4\n");
            continue;

        }




        else{

            write(fd, arg, strlen(arg));

        }
    }


    unlink(myfifo);
    close(fd);
    
    
    return 0;
        
    
 
    exit(EXIT_SUCCESS);
}

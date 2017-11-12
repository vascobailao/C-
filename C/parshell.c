	
// Par-shell
//
  
#include <stdlib.h>
#include <sys/wait.h>
#include <string.h>
#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <pthread.h>
#include "commandlinereader.h"
#include "list.h"
#include "exemplo.h"
#include "semaphore.h"
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>

#define SIZE 5
#define MAXPAR 10
#define MAX_BUFFER 1024
#define MAX 60
   
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
extern int nmr_processes;
int arrayPIDS[1000];

int contaiteracoes=0;
int totaltime=0; 
char filename[64];
void handler();
char args1[1000];




 
int main (int argc, char** argv){

    
    
    char *argvector[SIZE];
    int err;
    pthread_t tid = 1;
    list = lst_new();
    nChilds = 0;
    exit1 = 0;
    char buffer[MAX_BUFFER];
    int j=0;
    int nmr_processes;
    int ppid=0;

   signal(SIGINT, handler);

   int fd2;

        char * myfifo2 = "/tmp/parshell-in1";
        char buf[MAX_BUFFER];
        unlink(myfifo2);
        mkfifo(myfifo2, O_CREAT | O_WRONLY, S_IRUSR | S_IWUSR);
        fd2 = open(myfifo2,O_CREAT | O_WRONLY, S_IRUSR | S_IWUSR);

   
  
    char *string = (char*) malloc(sizeof(char)*60);
    int contadorlinhas=0;
    int i;


   FILE* fp;
   fp = fopen("log.txt" , "a+");
 

   
   while (fgets (string, 60, fp)!=NULL ) 
   {
      contadorlinhas++;
      
   }
   if(contadorlinhas>=3){
        
        contaiteracoes = (contadorlinhas/3);

    sscanf(string, "total time: %d", &i);
    
    totaltime=i;
}
   fclose(fp);
    

  pthread_cond_init(&semfilhos, NULL);
  pthread_cond_init(&semexec, NULL);
  err = pthread_create(&tid, NULL, &monitora, NULL);
        if (err != 0)
            printf("\ncan't create thread :[%s]", strerror(err));
        else
            printf("\nThread created successfully\n");


   int fd;
   char * myfifo = "/tmp/parshell-in";
   //char buf[MAX_BUFFER];

  if(access(myfifo, F_OK) == -1)
    if(mkfifo(myfifo, 0666)<0)
        exit(-1);

   fd = open(myfifo, O_RDONLY);
    
    close(0);
    dup(fd);
   

  while(1){
    
    int nargs = readLineArguments(argvector, SIZE);


    if(strcmp(argvector[0], "Processpid")==0) {
        
        arrayPIDS[ppid]=atoi(argvector[1]);                 
        ppid++;
        continue;
    }
    
    if(nargs==0){

        printf("Insira um argumento\n");
        free(argvector[0]);
        continue;
    }

    if (nargs==-1){
        free(argvector[0]);
        exit(EXIT_FAILURE);
    }
 
    else{ 
 
    int pid;
 
 
    if(strcmp(argvector[0],"exit-global") ==0){


        pthread_mutex_lock(&lock);
	    pthread_cond_signal(&semfilhos);
        exit1 = 1;
        for(j=0; arrayPIDS[j]!=0; j++) {
            kill(arrayPIDS[j], SIGINT);
        }

        pthread_mutex_unlock(&lock);
        pthread_join(tid, NULL);
        
       
        lst_destroy(list);
        pthread_cond_destroy(&semfilhos);
        pthread_cond_destroy(&semexec);
        free(argvector[0]);
        printf("Shell terminada.\n");
        exit(EXIT_SUCCESS);
  
    }

    if(strncmp(argvector[0],"stats",5) ==0){    

        printf("Entrei no stats\n");    
    
        //char args1[1000];
    
        //snprintf(args1, sizeof(args1), "Number of Processes: %d Total Time: %d\n", nChilds, totaltime);

        
        if (fd2 < 0)
        {
            printf("erro!\n");
            printf("Erro do open()! %s\n", strerror(errno));
        }


        printf("Entrei no stats1\n");  
        printf("%d\n", fd2);
        printf("Entrei no stats2\n");  
        write(fd2, "hi", sizeof("hi"));
        //printf("%s", args1);

        continue;
    }



    pthread_mutex_lock(&lock);
    
    while(nChilds==MAXPAR) pthread_cond_wait(&semexec,&lock);
 
    pthread_mutex_unlock(&lock);
    pid=fork();

    
    if (pid<0)
        perror("fork error");
     
    if(pid==0){
        
        int desc;
     
        snprintf(filename, MAX, "par-shell-out-%d.txt", (int) getpid());
        desc=open(filename, O_CREAT | O_WRONLY, S_IRUSR | S_IWUSR);
        printf("%d\n", desc);
        dup2(desc, 1);
        close(desc);
        nmr_processes++;
        execv(argvector[0],argvector);                                   
        perror ("Erro na criacao do processo filho: Path incorrecto\n");
        nmr_processes--;
        exit(EXIT_FAILURE);
  
    
    }
    else{
        /*Executa o código do processo pai*/
        
        pthread_mutex_lock(&lock);
        insert_new_process(list, pid, time(NULL));
       
        nChilds++;
        pthread_cond_signal(&semfilhos);
        pthread_mutex_unlock(&lock);
        free(argvector[0]);
        
        }
    }
  }
  exit(EXIT_SUCCESS);
}

void handler() {


    int j;
    for(j=0; arrayPIDS[j]!=0; j++) {
        kill(arrayPIDS[j], SIGINT);
    }
    signal(SIGINT, handler);
    

}



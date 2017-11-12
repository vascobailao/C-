#include <stdlib.h>
#include <stdio.h>
#include <sys/wait.h>
#include "list.h"
#include <time.h>
#include <pthread.h>
#include <unistd.h>
#include "exemplo.h"
 
extern list_t *list;
extern int status;
extern int nChilds;
extern int exit1;
extern int contaiteracoes;
extern pthread_mutex_t lock;
extern pthread_cond_t semexec;
extern pthread_cond_t semfilhos;
extern int totaltime;
extern int contaiteracoes;
 
void *monitora(void *arg)
{   
    int pidaux, i ;

    while(1){

        pthread_mutex_lock(&lock);

        while(nChilds==0 && exit1==0){


            pthread_cond_wait(&semfilhos,&lock);
            continue;
        }
        pthread_mutex_unlock(&lock);

 
         if (exit1==1 && nChilds==0){
            
         	
         	break;
         }
         	
 
     else{
        pthread_mutex_lock(&lock);
        for(i=0; i<nChilds; i++){
             pthread_mutex_unlock(&lock);

            
            pidaux = wait(&status);
            
            if (pidaux<0){
                continue;
            }
            
            if (WIFEXITED(status)){
                pthread_mutex_lock(&lock);

                update_terminated_process(list, pidaux, time(NULL), &contaiteracoes, &totaltime);
                nmr_processes--;

                
                nChilds--;

                pthread_cond_signal(&semexec);
                pthread_mutex_unlock(&lock);
            }
            else{
                pthread_mutex_lock(&lock);
      
                nChilds--;
                pthread_cond_signal(&semexec);
                pthread_mutex_unlock(&lock);
            }
         }
          
 
 
        }
         }
 
 
         
    
 
    pthread_exit(NULL);
}

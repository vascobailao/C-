/*
 * list.c - implementation of the integer list functions 
 */
 
 
#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include "list.h"
 extern FILE *fptr;

 
 
 
 
list_t* lst_new()
{
   list_t *list;
   list = (list_t*) malloc(sizeof(list_t));
   list->first = NULL;

   return list;
}
 
 
void lst_destroy(list_t *list)
{
    struct lst_iitem *item, *nextitem;
 
    item = list->first;
    while (item != NULL){
        nextitem = item->next;
        free(item);
        item = nextitem;
    }
    free(list);
}
 
 
void insert_new_process(list_t *list, int pid, time_t starttime)
{
    lst_iitem_t *item;
 
    item = (lst_iitem_t *) malloc (sizeof(lst_iitem_t));
    item->pid = pid;
    item->starttime = starttime;
    item->next = list->first;
    list->first = item;
}
 
void update_terminated_process(list_t *list, int pid, time_t endtime, int* iterador, int* totaltime)
{   
 
    lst_iitem_t *item = list->first;
    while(item!= NULL){
        if (item->pid==pid){
            item->endtime=time(NULL);
            
        fptr=(fopen("log.txt","a"));
        
   
        
        if(item->endtime!=0){
        (*iterador)++;;
        int tempo = (int)item->endtime-item->starttime;
        
        *totaltime+=tempo;
        fprintf(fptr,"iteração %d\n", *iterador );
        fprintf(fptr,"pid: %d\t execution time :%d s\n", item->pid, tempo);
        fprintf(fptr, "total time: %d \n", *totaltime);
        fflush(fptr);
        
      
         
        //item->iterador++;
        //printf("%d\n",item->iterador);
        }
        
        
        fclose(fptr);

        }

        item=item->next;
    }
   
}



  
 
/*void lst_print(list_t *list, int iterador, int totaltime)
{
    lst_iitem_t *item;
    item = list->first;
   
      
    while (item != NULL){
        
         
        printf("Iterador===%d\n",iterador);
        fptr=(fopen("log.txt","a"));
        
   
        
        if(item->endtime!=0){
        int tempo = (int)item->endtime-item->starttime;
        totaltime+=tempo;
        fprintf(fptr,"iteração %d\n", iterador );
        fprintf(fptr,"pid: %d\t execution time :%d s\n", item->pid, tempo);
        fprintf(fptr, "total time: %d \n", totaltime);
        fflush(fptr);
         iterador++;
         
        //item->iterador++;
        //printf("%d\n",item->iterador);
        }
        
        item = item->next;
        fclose(fptr);
    }
     
}*/
import { Component } from '@angular/core';
import {  List } from '../../../features/user/interfaces/list.interface';
import { DatePipe } from '@angular/common';
import { ItemListComponent } from '../../../features/user/components/item-list/item-list.component';

@Component({
  selector: 'lists-page',
  imports: [DatePipe, ItemListComponent],
  templateUrl: './lists-page.component.html',
})
export class ListsPageComponent { 
   lists: List[] = [
    {
      id: '1',
      title: 'The Top 1%',
      image: 'https://via.placeholder.com/80',
      creationDate: '2019-08-11',
      cant: 84,
      comments: 89,
      views: 590324,
      follows: 2723,
      author: 'Stesira',
      genres: ['No Tags'],
      description: "Sturgeon's law: 99% of everything is terrible..."
    },
    {
      id: '2',
      title: 'Sweet, Fluffy, MC Transmigration Into Book',
      image: 'https://via.placeholder.com/80',
      creationDate: '2019-08-06',
      cant: 100,
      comments: 15,
      views: 373998,
      follows: 1152,
      author: 'Alina Moktan',
      genres: ['Acting', 'Ancient Times', 'Modern Day', 'Cute Children'],
      description: 'If you are into sweet, funny and fluffy modern/Ancient Chinese novel with transmigration MC...'
    }
  ];
}

import { Component, input } from '@angular/core';
import { List } from '../../interfaces/list.interface';
import { Calendar, Eye, ListOrdered, LucideAngularModule, Pencil, User, Users, X } from 'lucide-angular';

@Component({
  selector: 'item-list',
  imports: [LucideAngularModule],
  templateUrl: './item-list.component.html',
})
export class ItemListComponent { 
  list = input<List>();

  // Iconos
  readonly user = User;
  readonly cant = ListOrdered;
  readonly view = Eye;
  readonly calendar = Calendar;
  readonly follow = Users;
  readonly edit = Pencil;
  readonly delete = X;
  
}


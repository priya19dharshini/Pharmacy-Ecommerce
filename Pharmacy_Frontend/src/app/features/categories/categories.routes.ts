import { Routes } from '@angular/router';

export const CATEGORY_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/category-list/category-list.component').then(
        component => component.CategoryListComponent
      )
  },
  {
    path: 'add',
    loadComponent: () =>
      import('./components/category-form/category-form.component').then(
        component => component.CategoryFormComponent
      )
  },
  {
    path: 'edit/:id',
    loadComponent: () =>
      import('./components/category-form/category-form.component').then(
        component => component.CategoryFormComponent
      )
  }
];

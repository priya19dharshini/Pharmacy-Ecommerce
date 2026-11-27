import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'categories',
    loadChildren: () =>
      import('./features/categories/categories.routes').then(
        route => route.CATEGORY_ROUTES
      )
  },
  {
    path: 'medicines',
    loadChildren: () =>
      import('./features/medicines/medicines.routes').then(
        route => route.MEDICINE_ROUTES
      )
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'categories'
  },
  {
    path: '**',
    redirectTo: 'categories'
  }
];

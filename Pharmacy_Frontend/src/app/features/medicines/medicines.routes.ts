import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const MEDICINE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/medicine-list/medicine-list.component').then(
        component => component.MedicineListComponent
      )
  },
  {
    path: 'add',
    canActivate: [roleGuard(['Admin'])],
    loadComponent: () =>
      import('./components/medicine-form/medicine-form.component').then(
        component => component.MedicineFormComponent
      )
  },
  {
    path: 'edit/:id',
    canActivate: [roleGuard(['Admin'])],
    loadComponent: () =>
      import('./components/medicine-form/medicine-form.component').then(
        component => component.MedicineFormComponent
      )
  },
  {
    path: 'details/:id',
    loadComponent: () =>
      import('./components/medicine-details/medicine-details.component').then(
        component => component.MedicineDetailsComponent
      )
  }
];

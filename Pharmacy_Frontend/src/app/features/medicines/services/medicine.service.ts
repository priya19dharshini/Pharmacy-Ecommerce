import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  CreateMedicine,
  Medicine,
  UpdateMedicine
} from '../models/medicine.model';

@Injectable({
  providedIn: 'root'
})
export class MedicineService {
  private readonly http = inject(HttpClient);
  private readonly medicinesUrl = `${environment.apiUrl}/medicines`;

  getAllMedicines(): Observable<Medicine[]> {
    return this.http.get<Medicine[]>(this.medicinesUrl);
  }

  getMedicineById(id: number): Observable<Medicine> {
    return this.http.get<Medicine>(`${this.medicinesUrl}/${id}`);
  }

  createMedicine(data: CreateMedicine): Observable<Medicine> {
    return this.http.post<Medicine>(this.medicinesUrl, data);
  }

  updateMedicine(id: number, data: UpdateMedicine): Observable<Medicine> {
    return this.http.put<Medicine>(`${this.medicinesUrl}/${id}`, data);
  }

  deleteMedicine(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.medicinesUrl}/${id}`
    );
  }
}

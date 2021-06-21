import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/Pagination";

export function getPaginationResult<T>(url: string , params : HttpParams , http: HttpClient) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return http.get<T>(url , {observe: 'response' , params}).pipe(
      map(response => {
        //console.log(response);
        paginatedResult.result = response.body; // tra ve mot Pagination.result cho client ts
       // console.log(response.body);
        if(response.headers.get('Pagination') !== null ){
           //parse Json va gán dữ liệu vào InterFace pageSize ,pageNumber v...v
//paginatedResult.pagination = {currentPage: 1, itemsPerPage: 5, totalItems: 3, totalPages: 1}
         paginatedResult.pagination = JSON.parse(response.headers.get('pagination'));   
         //console.log(paginatedResult.pagination) ;
        }       
        return paginatedResult;
      })
    )
  }

  export function getPaginationHeaders(pageNumber: number , pageSize: number ) {
    // gán giá trị và đưa vào router của http Api của trình duyệt nhu la tham số
    let params = new HttpParams(); 
    params = params.append('pageNumber' , pageNumber.toString());
    params = params.append('pageSize' , pageSize.toString());
    return params;
    // return this.httpClient.get<repos[]>(this.baseURL + 'users/' + userName + '/repos', {params});
  }
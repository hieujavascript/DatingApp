export interface Pagination { // thu tu o day ko quan trong
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}
export class PaginatedResult<T> {
    result : T;
    pagination: Pagination;
}

// // gán dữ liệu bằng 
// var PaginatedResult = new PaginatedResult();
// PaginatedResult.result = [ {a : 1 , b: 2} , {a: 3 , b: 4}]
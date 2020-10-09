[<AutoOpen>]
module DbExtensions

open System.Data.SqlClient
open Dapper

let getConnection (str) =
    async {
        let conn = new SqlConnection(str)
        let! _ = conn.OpenAsync() |> Async.AwaitTask
        return conn
    }

let execute (conn: SqlConnection) (sql: string) parameters =
    match parameters with
    | None -> conn.ExecuteAsync(sql) |> Async.AwaitTask
    | Some p -> conn.ExecuteAsync(sql, p) |> Async.AwaitTask

let query<'T> (conn: SqlConnection) (sql: string) parameters =
    async {
        let! results =
            match parameters with
            | None -> conn.QueryAsync<'T>(sql) |> Async.AwaitTask
            | Some p -> conn.QueryAsync<'T>(sql, p) |> Async.AwaitTask

        return List.ofSeq results
    }

let querySingle<'T> (conn: SqlConnection) (sql: string) parameters =
    async {
        let! result = query<'T> conn sql parameters
        return List.tryHead result
    }

let (!) p = Some(box p)

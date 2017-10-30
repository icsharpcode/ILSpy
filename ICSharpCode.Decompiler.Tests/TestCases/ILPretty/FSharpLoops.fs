open System

let disposable() = { new IDisposable with member x.Dispose() = () }

let getSeq() = seq { yield 1; }
let getList() = [ 1 ]
let getArray() = [| 1 |]

[<EntryPoint>]
let main argv =

    // nested using scopes?
    use disp1 =
        use disp2 = disposable()
        Console.WriteLine "Hello 1"
        disposable()
    
    // for loop over seq
    for i in getSeq() do
        Console.WriteLine i
        
    // for loop over list
    for i in getList() do
        Console.WriteLine i
        
    // for loop over array
    for i in getArray() do
        Console.WriteLine i
        
    0 // return an integer exit code
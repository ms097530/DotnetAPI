function test1()
{
    console.log('test1')
    test2()
}
function test2()
{
    console.log('test2')
    test3()
}
function test3()
{
    throw new Error('error in test 3')
}

try
{
    test1();
}
catch (e)
{
    console.log(e)
}

console.log('after the error')
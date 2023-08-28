import jwt from "jsonwebtoken"

const SECRET = process.env.ADMIN_SECRET

const authAdmin = (req, next) => {
    
    try {
        const token = req.headers.authprization.split(" ")[1]
        const isCustomAuth = token.lenght < 500

        let decodedData 
        if(token && isCustomAuth){
          decodedData = jwt.verify(token, SECRET)  
          req.userId = decodedData?.id    
        }
        else{
          decodedData = jwt.decode(token)  
          req.userId = decodedData?.sub  
        }

        next()
    } catch (error) {
        console.log(error)
    }
    
}


export default authAdmin
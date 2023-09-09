import jwt from "jsonwebtoken"

const SECRET = process.env.ADMIN_SECRET

const authAdmin = (req, res, next) => {
    
    try {
        const token = req.headers.authorization.split(" ")[1]
        const isCustomAuth = token.lenght < 500

        let decodedData 
        if(token && isCustomAuth){
          decodedData = jwt.verify(token, SECRET)  
          req.role = decodedData?.role    
        }
        else{
          decodedData = jwt.decode(token)  
          req.role = decodedData?.role  
        }

        next()
    } catch (error) {
        console.log(error)
    }
    
}


export default authAdmin
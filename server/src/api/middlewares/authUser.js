import jwt from "jsonwebtoken"

const authUser = (req, res, next) => {
    
    try {
        const token = req.headers.authorization.split(" ")[1]

        const isCustomAuth = token.length < 500
        let decodedData 

        if(token && isCustomAuth){
          const SECRET = process.env.USER_SECRET;
  
          decodedData = jwt.verify(token, SECRET)  
          req.email = decodedData?.email    
        }
        else{
          decodedData = jwt.decode(token)  
          req.email = decodedData?.email  
        }

        next()
    } catch (error) {
        console.log(error)
    }
    
}


export default authUser
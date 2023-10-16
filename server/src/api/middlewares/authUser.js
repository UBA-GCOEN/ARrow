import jwt from "jsonwebtoken"

const authUser = (req, res, next) => {
    
    try {
        const token = req.headers.authorization.split(" ")[1]

        const isCustomAuth = token.length < 500
        let decodedData 

        if(token && isCustomAuth){
          let SECRET = process.env.USER_SECRET
          // if(req.session.user){
          //  SECRET = process.env.USER_SECRET
          // }
          // else{
          //   res.json({msg: "Not authorized - login first"})
          // }

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
import jwt from "jsonwebtoken"

var SECRET = ''

const authAdmin = (req, res, next) => {
    
    try {
        const token = req.headers.authorization.split(" ")[1]
        const isCustomAuth = token.length < 500
        let decodedData 
        if(token && isCustomAuth){
          if(req.session.user.user.role === 'admin'){
            SECRET = process.env.ADMIN_SECRET
          }
          else if(req.session.user.user.role === 'faculty'){
            SECRET = process.env.FACULTY_SECRET
          }
          else if(req.session.user.user.role === 'staff'){
            SECRET = process.env.STAFF_SECRET
          }
          else if(req.session.user.user.role === 'student'){
            SECRET = process.env.STUDENT_SECRET
          }
          else if(req.session.user.user.role === 'visitor'){
            SECRET = process.env.VISITOR_SECRET
          }
  
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
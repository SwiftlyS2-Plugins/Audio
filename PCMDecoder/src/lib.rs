use mimalloc::MiMalloc;

pub mod decoder;


#[global_allocator]
static GLOBAL: MiMalloc = MiMalloc;


#[cfg(test)]
mod tests {

    

    

}